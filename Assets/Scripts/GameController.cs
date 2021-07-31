using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public GameObject cubeToCreate, //Префаб куба, который нужно создать
                      allCubes; // Игровой объект всех кубов
    public GameObject[] canvasStartPage; //Главное меню
    public Color[] bgColors; //Цвета для заднего фона
    public Transform cubeToPlace; //Место, куда можно поставить куб
    private Rigidbody allCubesRB; //Физика все кубов
    private bool isLose, //Проиграл ли игрок
                firstCube; //Поставлен ли первый куб
    private Color toCameraColor; //Текущий свет заднего фона
    private float moveCameraToYPosition, //На какой координате по Y находится камера 
                camMoveSpeed = 2f; //Скорость камеры
    private Coroutine showCubePlace; //Показывает, куда можно куб
    private int prevCountMaxHorizontal; //Макс. кол-во блоков в сторону
    private Transform mainCam; //Главная камера
    private CubePos nowCube = new CubePos(0, 1, 0); //Расположение текущего куба
    public float cubeChangePlaceSpeed = 0.5f; //Скорость выбора смены положения нового куба
    private List<Vector3> allCubesPositions = new List<Vector3>{ //Расположение всех кубов
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
    };
    
    private void Start()
    {
        toCameraColor = Camera.main.backgroundColor; //Указываем какой сейчас цвет заднего фона

        mainCam = Camera.main.transform; //Указываем главную камеру
        moveCameraToYPosition = 7.12f + nowCube.y - 1f; //Ставим камеру на место

        allCubesRB = allCubes.GetComponent<Rigidbody>(); //Создаем физику для всех кубов
        showCubePlace = StartCoroutine(ShowCubePlace()); //Стартуем корутину
    }

    private void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject()){ //Если нажали на экран
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began){return;} //Если удерживают палец, то не создаем куб
#endif

            if(!firstCube){ //Проверка на то, начата ли игра
                firstCube = true; // для того, чтобы убрать кнопки
                foreach (GameObject obj in canvasStartPage){
                    Destroy(obj);
                }
            }

            GameObject newCube = Instantiate( //Создаем объект кубу
                cubeToCreate,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;
            
            newCube.transform.SetParent(allCubes.transform); //Устанавливаем кубу родителя
            nowCube.SetVector(cubeToPlace.position); //Ставим куб на свое место
            allCubesPositions.Add(nowCube.GetVector()); //Добавляем куб в список всех кубов

            allCubesRB.isKinematic = true; //Здесь мы "обновляем" физику всех кубов 
            allCubesRB.isKinematic = false;//

            SpawnPosition(); //Запускаем спавн
            moveCameraChangeBg(); // Вращаем камеру и меняем фон
        }

        if(allCubesRB.velocity.magnitude > 0.1f && !isLose){ //Если башня пошатнулась -  игрок проиграл
            Destroy(cubeToPlace.gameObject);
            isLose = true;
            StopCoroutine(showCubePlace);
            Camera.main.transform.localPosition = Vector3.Lerp(a: Camera.main.transform.localPosition,
                b: new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, Camera.main.transform.localPosition.z - 10f),
                t: 50f / Time.deltaTime);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,                        //
            new Vector3(mainCam.localPosition.x, moveCameraToYPosition, mainCam.localPosition.z), //Вешаем камеру выше, в зависимости от Y
            camMoveSpeed * Time.deltaTime);                                                       //

        if(Camera.main.backgroundColor != toCameraColor){ //Меняем фон
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);
        }
    }

    private void moveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;
        
        foreach (Vector3 pos in allCubesPositions){ //Перебераем все кубы
            if (Math.Abs(Convert.ToInt32(pos.x)) > maxX){ //Ищем макс. на X
                maxX = Convert.ToInt32(pos.x);}

            if (Math.Abs(Convert.ToInt32(pos.y)) > maxY){ //Ищем макс. на y
                maxY = Convert.ToInt32(pos.y);}

            if (Math.Abs(Convert.ToInt32(pos.z)) > maxZ){ //Ищем макс. на z
                maxZ = Convert.ToInt32(pos.z);}
        }

        moveCameraToYPosition = 7.12f + nowCube.y - 1f; //Меняем высоту камеры

        maxHor = maxX > maxZ ? maxX : maxZ; // Ищем макс. на всей горизонтале
        if(maxHor % 3 == 0 && prevCountMaxHorizontal != maxHor){ //Каждые 3 блока увеличиваем дальность зрения камеры
            mainCam.localPosition += new Vector3(0, 0, -2.5f);
            prevCountMaxHorizontal = maxHor;
        }

        if(maxY <= 2){                   //
            toCameraColor = bgColors[0]; //
        }else if (maxY <= 6){            //
            toCameraColor = bgColors[1]; //
        }else if (maxY <= 9){            //Меняем фон, в зависимости от высоты
            toCameraColor = bgColors[2]; //
        }else if (maxY <= 12){           //
            toCameraColor = bgColors[2]; //
        }else if (maxY <= 15){           //
            toCameraColor = bgColors[2]; //
        }
    }


    IEnumerator ShowCubePlace() //Постоянно создаем позицию для нового блока
    {
        while(true){
            SpawnPosition();

            yield return new WaitForSeconds(cubeChangePlaceSpeed); //Ждем перед созданием новой позиции
        }
    }
    
    private void SpawnPosition(){ //Создаем возможные позиции
        List<Vector3> positions = new List<Vector3>();

        if(IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x +1 != cubeToPlace.position.x){ // Если на этом месте пусто, то создаем возможную позицию
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));}

        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x){
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));}

        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y){
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));}
            
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y){
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));}

        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z){
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));}
        
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z){
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        }

        if(positions.Count > 1){ //Выбираем рандомное место показа нового блока
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        }else if(positions.Count == 0){ //Если некуда поставить блок-ты проиграл
            isLose = true;
        }else{ //Если возможный вариант всего один то
            cubeToPlace.position = positions[0];
        }
    }

    private bool IsPositionEmpty(Vector3 targetPos){ //Проверяем, пуста ли позиция
        if(targetPos.y == 0){return false;}

        foreach (Vector3 pos in allCubesPositions){
            if(pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z){return false;}  
        }

        return true;
    }
}

struct CubePos{ 
    public int x, y, z;

    public CubePos(int x, int y, int z){ //Конструктор 
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVector(){ //Получение позиции
        return new Vector3(x,y,z);
    }

    public void SetVector(Vector3 pos){ //Задаем позицию
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}

//ЧТО ЭТО???
internal struct NewStruct
{
    public int Item1;
    public int Item2;
    public int Item3;

    public NewStruct(int item1, int item2, int item3)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
    }

    public override bool Equals(object obj)
    {
        return obj is NewStruct other &&
               Item1 == other.Item1 &&
               Item2 == other.Item2 &&
               Item3 == other.Item3;
    }

    public override int GetHashCode()
    {
        int hashCode = 341329424;
        hashCode = hashCode * -1521134295 + Item1.GetHashCode();
        hashCode = hashCode * -1521134295 + Item2.GetHashCode();
        hashCode = hashCode * -1521134295 + Item3.GetHashCode();
        return hashCode;
    }

    public void Deconstruct(out int item1, out int item2, out int item3)
    {
        item1 = Item1;
        item2 = Item2;
        item3 = Item3;
    }

    public static implicit operator (int, int, int)(NewStruct value)
    {
        return (value.Item1, value.Item2, value.Item3);
    }

    public static implicit operator NewStruct((int, int, int) value)
    {
        return new NewStruct(value.Item1, value.Item2, value.Item3);
    }
}