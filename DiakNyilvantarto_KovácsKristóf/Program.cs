using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiakNyilvantarto_KovácsKristóf
{
    internal class Program
    {
        
        static string connectionString = "Server=localhost;Database=diak_gyak;Uid=root;Pwd=;";

        static void Main(string[] args)
        {
            bool fut = true;

            while (fut)
            {
                Console.Clear();
                Console.WriteLine("===== DIÁKNYILVÁNTARTÓ =====\n");
                Console.WriteLine("1. Diákok listázása");
                Console.WriteLine("2. Új diák felvétele");
                Console.WriteLine("3. Diák adatainak módosítása");
                Console.WriteLine("4. Diák törlése");
                Console.WriteLine("5. Kilépés\n");
                Console.Write("Választás: ");

                string valasztas = Console.ReadLine();
                Console.WriteLine();

                switch (valasztas)
                {
                    case "1":
                        DiakokListazasa();
                        break;
                    case "2":
                        UjDiak();
                        break;
                    case "3":
                        DiakModositasa();
                        break;
                    case "4":
                        DiakTorlese();
                        break;
                    case "5":
                        fut = false;
                        Console.WriteLine("Kilépés a programból...");
                        break;
                    default:
                        Console.WriteLine("Hibás menüpont! Kérlek, 1 és 5 közötti számot adj meg.");
                        break;
                }

                if (fut)
                {
                    Console.WriteLine("\nNyomj meg egy billentyűt a főmenübe való visszatéréshez...");
                    Console.ReadKey();
                }
            }
        }

        static void DiakokListazasa()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT id, nev, eletkor, osztaly FROM diakok";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"{"ID",-5} {"Név",-20} {"Kor",-6} {"Osztály",-10}");
                        Console.WriteLine(new string('-', 42));

                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string nev = reader.GetString("nev");
                            int eletkor = reader.GetInt32("eletkor");
                            string osztaly = reader.GetString("osztaly");

                            Console.WriteLine($"{id,-5} {nev,-20} {eletkor,-6} {osztaly,-10}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba történt a listázás során: " + ex.Message);
            }
        }

        static void UjDiak()
        {
            Console.WriteLine("Új diák felvétele\n");

            Console.Write("Név: ");
            string nev = Console.ReadLine();

            Console.Write("Életkor: ");
            if (!int.TryParse(Console.ReadLine(), out int eletkor))
            {
                Console.WriteLine("Hibás életkor formátum! Csak számot adhatsz meg.");
                return;
            }

            Console.Write("Osztály: ");
            string osztaly = Console.ReadLine();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO diakok (nev, eletkor, osztaly) VALUES (@nev, @eletkor, @osztaly)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nev", nev);
                        cmd.Parameters.AddWithValue("@eletkor", eletkor);
                        cmd.Parameters.AddWithValue("@osztaly", osztaly);

                        int erintettSorok = cmd.ExecuteNonQuery();

                        if (erintettSorok > 0)
                        {
                            Console.WriteLine("\n1 diák sikeresen felvéve!");
                        }
                        else
                        {
                            Console.WriteLine("\nNem sikerült a diák felvétele.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nHiba történt a felvétel során: " + ex.Message);
            }
        }

        static void DiakModositasa()
        {
            Console.WriteLine("Diák módosítása\n");

            Console.Write("Módosítandó diák ID-ja: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Hibás ID formátum!");
                return;
            }

            Console.Write("Új név: ");
            string ujNev = Console.ReadLine();

            Console.Write("Új életkor: ");
            if (!int.TryParse(Console.ReadLine(), out int ujEletkor))
            {
                Console.WriteLine("Hibás életkor formátum!");
                return;
            }

            Console.Write("Új osztály: ");
            string ujOsztaly = Console.ReadLine();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE diakok SET nev = @nev, eletkor = @eletkor, osztaly = @osztaly WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nev", ujNev);
                        cmd.Parameters.AddWithValue("@eletkor", ujEletkor);
                        cmd.Parameters.AddWithValue("@osztaly", ujOsztaly);
                        cmd.Parameters.AddWithValue("@id", id);

                        int erintettSorok = cmd.ExecuteNonQuery();

                        if (erintettSorok > 0)
                        {
                            Console.WriteLine("\nDiák módosítva!");
                        }
                        else
                        {
                            Console.WriteLine("\nNem található diák a megadott azonosítóval!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nHiba történt a módosítás során: " + ex.Message);
            }
        }

        static void DiakTorlese()
        {
            Console.WriteLine("Diák törlése\n");

            Console.Write("Törlendő diák ID-ja: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Hibás ID formátum!");
                return;
            }

            Console.Write($"Biztosan törölni szeretnéd a(z) {id} azonosítójú diákot? (i/n): ");
            string megerosites = Console.ReadLine()?.ToLower();

            if (megerosites != "i" && megerosites != "igen")
            {
                Console.WriteLine("Törlés megszakítva.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM diakok WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        int erintettSorok = cmd.ExecuteNonQuery();

                        if (erintettSorok > 0)
                        {
                            Console.WriteLine("\nDiák sikeresen törölve!");
                        }
                        else
                        {
                            Console.WriteLine("\nNem található diák a megadott azonosítóval!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nHiba történt a törlés során: " + ex.Message);
            }
        }

    }
}
