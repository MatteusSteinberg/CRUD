using System;
using System.Data.SqlClient;
using System.Data;

namespace MSSQLProject
{
    class Program
    {
        // Variable til at kalde på Tabel
        static readonly string tableName = "Library";
        // Connection string til at connect til SQL Serveren
        static readonly string ConnectionString = "Data source=(local);Initial Catalog = MSSQLProject;Integrated Security=SSPI";
        static void Main(string[] args)
        {
            //Hele er i en While loop så den kan komme tilbage til start menuen efter den har kørt funktionen
            while (true)
            { 
                int chosen;
                // Start menu
                Console.WriteLine("What book shall you read today?\n\nChoose one of the following:\n1. Explore the library\n2. Add a Book\n3. Delete a Book\n4. Update a Book\n ");
                chosen = Convert.ToInt32(Console.ReadLine());
                // Nummer man vælger til at bruge funktion
                if (chosen == 1)
                {
                    Library();
                }

                else if (chosen == 2)
                {
                    addBook();
                }
                else if (chosen == 3)
                {
                    delBook();
                }
                else if (chosen == 4)
                {
                    updBook();
                }
                else
                {
                    Console.WriteLine("How about you go to bed instead?");
                }

            }

        }

        //laver en funktion som kan kalde på tabel
        static DataTable getTable()
        {
            DataTable Library = new DataTable();
            try
            {
                // laver en variable som man kan connect til serveren
                using (SqlConnection servConn = new SqlConnection(ConnectionString)) 
                {
                    //laver en SQL Komando som indeholder en SQL Query
                    SqlCommand cmd = new SqlCommand("select * from " + tableName, servConn);
                    //Omfomartere en SQL database som kan læses af C#
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    adpt.Fill(Library);
                }
                return Library;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
                return null;
            }
        }
        static void Library()
        {
            //Laver en funktion som udfylder hver række
            foreach (DataRow row in getTable().Rows)
            {
                string outputString = "Id: {0} | - Name: {1}\tAuthor: {2}\t DOR: {3}\t Genre: {4}";
                string formatting = string.Format(outputString,
                    row.ItemArray[0],
                    row.ItemArray[1],
                    row.ItemArray[2],
                    row.ItemArray[3].ToString(),
                    row.ItemArray[4]
                    );
                Console.WriteLine(formatting);
            }
        }

        static void addBook()
        {
            //Variabler som bliver brugt senere
            string NAME;
            string AUTHOR;
            string DOR;
            string GENRE;
            //Menu
            Console.Write("Book Name: ");
            NAME = Console.ReadLine();
            Console.Write("Author: ");
            AUTHOR = Console.ReadLine();
            Console.Write("Date Of Release: ");
            DOR = Console.ReadLine();
            Console.Write("Genre: ");
            GENRE = Console.ReadLine();

            SqlConnection servConn = new SqlConnection(ConnectionString);
            //Query som kan læses af SQL serveren
            string query = "INSERT INTO " + tableName + "(NAME, AUTHOR, DOR, GENRE)"+
            " VALUES(@NAME, @AUTHOR, @DOR, @GENRE)";

            SqlCommand cmd = new SqlCommand(query, servConn);
            //Definere hvilken type kolonnen er
            cmd.Parameters.AddWithValue("@NAME", SqlDbType.Text);
            cmd.Parameters.AddWithValue("@AUTHOR", SqlDbType.Text);
            cmd.Parameters.AddWithValue("@DOR", SqlDbType.Text);
            cmd.Parameters.AddWithValue("@GENRE", SqlDbType.Text);
            //Giver kolonnen en value som er variablen
            cmd.Parameters["@NAME"].Value = NAME;
            cmd.Parameters["@AUTHOR"].Value = AUTHOR;
            cmd.Parameters["@DOR"].Value = DOR;
            cmd.Parameters["@GENRE"].Value = GENRE;

            try
            {
                //Der åbnes en forbindelse til SQL serveren
                cmd.Connection.Open();
                //Queryen bliver kørt
                int rowsaffected = cmd.ExecuteNonQuery();
                //Forbindelsen bliver lukket
                cmd.Connection.Close();
                Console.WriteLine(rowsaffected.ToString() + " Book has been added!");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        static public void delBook()
        {
            int ID;
            //Library funktionen bliver kørt som det første
            Library();

            Console.Write("\nChoose the ID of the book that you want to Delete: ");
            ID = Convert.ToInt32(Console.ReadLine());

            string query = "DELETE FROM " + tableName + " WHERE Id = " + ID;

            SqlConnection servConn = new SqlConnection(ConnectionString);
            try
            {
                using SqlCommand cmd = new SqlCommand(query, servConn);
                servConn.Open();
                cmd.ExecuteNonQuery();
                servConn.Close();

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }    
        }

        static public void updBook()
        {
            int choice;
            int ID;

            Library();

            //Menu
            Console.WriteLine("Which ID row would you like to update?: ");
            ID = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("\nWhich of the following column would you like to update?\n1. All\n2. Book Name\n3. Author\n4. Date Of Release\n5. Genre");
            choice = Convert.ToInt32(Console.ReadLine());

            SqlConnection servConn = new SqlConnection(ConnectionString);

            if (choice == 1)
            {
                string NAME;
                string AUTHOR;
                string DOR;
                string GENRE;
                //Menu
                Console.Write("New Book Name: ");
                NAME = Console.ReadLine();
                Console.Write("New Author Name: ");
                AUTHOR = Console.ReadLine();
                Console.Write("New Date Of Release: ");
                DOR = Console.ReadLine();
                Console.Write("New Genre: ");
                GENRE = Console.ReadLine();

                string query = "UPDATE " + tableName + " SET NAME = @NAME, AUTHOR = @AUTHOR, DOR = @DOR, GENRE = @GENRE WHERE Id =" + ID.ToString();
                try
                {
                    using SqlCommand cmd = new SqlCommand(query, servConn);
                    cmd.Parameters.AddWithValue("@NAME",SqlDbType.Text);
                    cmd.Parameters.AddWithValue("@AUTHOR", SqlDbType.Text);
                    cmd.Parameters.AddWithValue("@DOR", SqlDbType.Text);
                    cmd.Parameters.AddWithValue("@GENRE", SqlDbType.Text);

                    cmd.Parameters["@NAME"].Value = NAME;
                    cmd.Parameters["@AUTHOR"].Value = AUTHOR;
                    cmd.Parameters["@DOR"].Value = DOR;
                    cmd.Parameters["@GENRE"].Value = GENRE;

                    servConn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    servConn.Close();

                    Console.Clear();
                    Console.WriteLine(affected.ToString() + " Book Has Been Updated!");


                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }


                Console.ReadKey();
            }
            else if (choice == 2)
            {
                string NAME;

                Console.Write("New Book Name: ");
                NAME = Console.ReadLine();

                string query = "UPDATE " + tableName + " SET NAME = @NAME WHERE Id =" + ID.ToString();
                try
                {
                    using SqlCommand cmd = new SqlCommand(query, servConn);
                    cmd.Parameters.AddWithValue("@NAME", SqlDbType.Text);

                    cmd.Parameters["@NAME"].Value = NAME;

                    servConn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    servConn.Close();

                    Console.Clear();
                    Console.WriteLine(affected.ToString() + " Book Has Been Updated!");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else if (choice == 3)
            {
                string AUTHOR;
                Console.Write("New Author Name: ");
                AUTHOR = Console.ReadLine();

                string query = "UPDATE " + tableName + " SET AUTHOR = @AUTHOR WHERE Id =" + ID.ToString();
                try
                {
                    using SqlCommand cmd = new SqlCommand(query, servConn);
                    cmd.Parameters.AddWithValue("@AUTHOR", SqlDbType.Text);

                    cmd.Parameters["@AUTHOR"].Value = AUTHOR;

                    servConn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    servConn.Close();

                    Console.Clear();
                    Console.WriteLine(affected.ToString() + " Book Has Been Updated!");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else if (choice == 4)
            {
                string DOR;

                Console.Write("New Date Of Release: ");
                DOR = Console.ReadLine();

                string query = "UPDATE " + tableName + " SET DOR = @DOR WHERE Id =" + ID.ToString();
                try
                {
                    using SqlCommand cmd = new SqlCommand(query, servConn);
                    cmd.Parameters.AddWithValue("@DOR", SqlDbType.Text);
                    cmd.Parameters["@DOR"].Value = DOR;

                    servConn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    servConn.Close();

                    Console.Clear();
                    Console.WriteLine(affected.ToString() + " Book Has Been Updated!");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else if (choice == 5)
            {
                string GENRE;
                Console.Write("New Genre: ");
                GENRE = Console.ReadLine();

                string query = "UPDATE " + tableName + " SET GENRE = @GENRE WHERE Id =" + ID.ToString();
                try
                {
                    using SqlCommand cmd = new SqlCommand(query, servConn);
                    cmd.Parameters.AddWithValue("@GENRE", SqlDbType.Text);
                    cmd.Parameters["@GENRE"].Value = GENRE;

                    servConn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    servConn.Close();

                    Console.Clear();
                    Console.WriteLine(affected.ToString() + " Book Has Been Updated!");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else
            {
                Console.WriteLine("sike, thats the wrong number");
                Console.ReadKey();

            }
        }

    }
}
