/* Wykonal Marcin Wiatr
 * 
 * Zaimplementuj grę w życie. Załóżmy że mamy prostokątną tablicę o wymiarach n × m (n i m są parametrami programu). 
 * Każda komórka może być w jednym z dwóch stanów: żywa ”X” lub martwa ”.”. 
 * Przyjmijmy, że komórki z prawej krawędzi siatki sąsiadują z komórkami z lewej krawędzi siatki, a komórki z górnego wiersza sąsiadują z komórkami dolnego wiersza siatki. 
 * Każda komórka ma 8 sąsiadów, połączonych z nią bokiem lub wierzchołkiem. 
 * Układ komórek podlega ewolucji. 
 * W następnym pokoleniu będą żywe tylko te komórki, które w bieżącym pokoleniu spełniają jeden z dwóch warunków: 
 * 
 * Komórka jest żywa i ma dwóch lub trzech żywych sąsiadów (inaczej umiera z samotności lub na skutek zbyt dużego zagęszczenia). 
 * Komórka jest martwa, ale ma dokładnie trzech żywych sąsiadów.  
 * 
 * Podaj liczbę żywych sąsiadów dla komórki w drugim wierszu i dziewiętnastej kolumnie w trzydziestym siódmym pokoleniu.
 * W którym pokoleniu (sprawdzamy maksymalnie do 100) układ żywych i martwych komórek się ustali (w bieżącym pokoleniu jest identyczny jak w poprzednim)? Podaj, które to pokolenie oraz liczbę żywych komórek w tym pokoleniu.
 * __________________________________________________________________________________________________________________________________________________________________________________________________________________________
 *  
 * Uwagi:
 * Zdecydowalem sie uzyc osobnej klasy do implementacji matrycy, moze to wspomoc rozbudowe programu oraz uzycie tych samych mechanizmow w wielu miejscach.
 * Zlozonosc czasowa i algorytmu jest dosc duza, O(n^2) w zwiazku z tym zdecydowalem sie na kilka optymalizacji, mozna sie takze zastanowic troche nad optymalizacja pamieciowa.
 * 
 * Jedna z optymalizacji jest szukanie "zywych" sasiadow danej komorki w CountAliveNeighbours(int x, int y), pierwotny sposob jaki zaimplementowalem polegal na bezposrednim wypisaniu konkretnych komorek do sprawdzenia,
 * tutaj odbywa sie to w petli po sasiednich komorkach. Glownym problemem byly komorki brzegowe, ktorych wartosc byla ujemna badz wieksza od rozmiaru tablicy. Aby ominac bledy wykorzystalem mod %, do "przesowania" 
 * indeksow komorek na przeciwna strone tablicy. Kiedys pisalem program ktory mial za zadanie przesowac napis jak na wyswietlaczach LED, z tad zaczerpnalem pomysl.
 * 
 * Kolejna z optymalizacji uzylem podczas wyszukiwania momentu w ktorym "zycie " sie stabilizuje. Uzylem Listy do przeniesienia wartosci tablic, i wykorzystalem Enumerable.SequenceEqual z Sysyem.Linq do porownania ich zgodnosci. 
 * 
 * Zastanawialem sie takze nad zastapieniem tablicy 2d, na List<T> lub Hashtable. Mysle ze mogloby to jeszcze bardziej zoptymalizowac czas jak i pamiec.
 *
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Life
{
    
    class Life
    {
        public static Matrix matrix = new Matrix(12, 20);//Inicjalizacja obiektu klasy Matrix z parametrami m i n
        static void Main(string[] args)
        {

            matrix.Alive(4, 7);//Inicjalizacja komorek poczatkowych
            matrix.Alive(4, 9);
            matrix.Alive(4, 10);

            matrix.Alive(5, 7);
            matrix.Alive(5, 8);
            matrix.Alive(5, 9);

            matrix.Alive(6, 8);

            bool getOut = false;//zmienna pomocnicza do wyjscia z petli

            for (int i = 1; i < 100; i++)//Zakladamy petle wykonywana 100 razy
            {

                int alive = 0;//zmienne pomocnicze
                int dead = 0;
                
                matrix.Print(50);//wywolanie metody do wyswietlenia pokolenia, czas wyswietlenia 50 ms                  
                if (i == 37)//Odpowiedz na pytanie 1
                {
                    Console.WriteLine($"Liczba zywych sasiadow dla komórki w 2 wierszu i 19 kolumnie w {i} pokoleniu wynosi: {matrix.CountAliveNeighbours(1, 18)}");
                    Console.WriteLine("Kontynuj...nacisnji klawisz");
                    Console.ReadKey();
                }

                List<int> list1 = new List<int>();//inicjalizacja tymczasowej listy do porownania
                foreach (var item in matrix.table)
                {
                    if (item == 1) alive++;
                    if (item == 0) dead++;
                    list1.Add(item);
                }
               
                matrix.Step();//wywolanie metody z klasy Matrix, inicjalizacja kolejnego pokolenia

                List<int> list2 = new List<int>();//inicjalizacja tymczasowej listy do porownania
                foreach (var item in matrix.table)
                {
                    list2.Add(item);
                }

                while ((list1.SequenceEqual(list2)) && ((getOut == false)))
                {
                    Console.WriteLine($"...Life is Still...Zycie  ustabilizowalo sie w {i} pokoleniu, matwych komorek {dead}, zywych {alive}.");
                    Console.WriteLine("Kontynuj...nacisnji klawisz");
                    Console.ReadKey();                   
                    getOut = true;
                }
                
                //Console.WriteLine(i);
                Console.Clear();
            }         
            Console.ReadKey();
        }
    }

    public class Matrix
    {
        public int row;//pole szerokosc, kierunek x
        public int col;//pole wysokosc, kierunek y
        public int[,] table;//definicja pola tablicy

        public Matrix(int row, int col)//Konstruktor
        {
            this.row = row;
            this.col = col;
            this.table = new int[row, col];
        }
        public void Print(int timeout = 100)//Metoda wyswietla tablice
        {
            for (int x = 0; x < row; x++)//wiersz
            {
                string line = "";
                for (int y = 0; y < col; y++)//kolumna
                {
                    if (this.table[x, y] == 1)//przypisanie znakow do komorek                              
                    {
                        line += "X";//zywa 
                    }
                    else
                    {
                        line += ".";//martwa 
                    }
                }
                Console.WriteLine(line);
            }
            Thread.Sleep(timeout);
        }
        public void Alive(int x, int y)//Oznaczenie komorek
        {
            this.table[x, y] = 1;//zywa: 1 
        }
        public void Dead(int x, int y)
        {
            this.table[x, y] = 0;//martwa: 0
        }
        public int CountAliveNeighbours(int x, int y)//Metoda zwraca sume zywych sasiadow
        {
            int sum = 0;

            for (int i = -1; i < 2; i++)//pela w lewo i prawo
            {
                for (int j = -1; j < 2; j++)//petla gora, dol
                {
                    int w = (x + i + row) % row;//modulo zwraca warosc indeksu komorki po drugiej stronie wiersza i kolumny dla wartosci skrajnych, dla wszystkich posrednich wartosci nic sie nie zmnienia,  
                    int h = (y + j + col) % col;//to pozwala nam "zawinac" brzegi prostokata.  

                    sum += table[w, h];
                }
            }
            sum -= table[x, y];//ta komorke rozpatrujemy wiec nie biore jej pod uwage


            return sum;
        }
        public void Step()//Kolejne pokolenie
        {
            int[,] newBoard = new int[row, col];//nowa tymczasowa tablica

            for (int x = 0; x < row; x++)
            {
                for (int y = 0; y < col; y++)
                {
                    int aliveNeighbours = CountAliveNeighbours(x, y);//ilosc zywych sasiadow

                    if (GetState(x, y) == 1)//warunek  Komórka jest żywa 
                    {
                        if (aliveNeighbours < 2)//ma mniej niz 2 sasiadow
                        {
                            newBoard[x, y] = 0;//umiera z samotności 
                        }
                        else if (aliveNeighbours == 2 || aliveNeighbours == 3)//komórka jest żywa i ma dwóch lub trzech żywych sąsiadów
                        {
                            newBoard[x, y] = 1;//jest dalej zywa
                        }
                        else if (aliveNeighbours > 3)//ma wiecej niz 3 sasiadow 
                        {
                            newBoard[x, y] = 0;//umiera na skutek zbyt dużego zagęszczenia
                        }
                    }
                    else
                    {
                        if (aliveNeighbours == 3)//Komórka jest martwa, ale ma dokładnie trzech żywych sąsiadów.
                        {
                            newBoard[x, y] = 1;
                        }
                    }

                }
            }

            this.table = newBoard;//tablica tymczasowa, poddmieniona
        }

        public int GetState(int x, int y)//Metoda wskazuje watrosc komorki
        {
            return this.table[x, y];
        }
    }

}
    

