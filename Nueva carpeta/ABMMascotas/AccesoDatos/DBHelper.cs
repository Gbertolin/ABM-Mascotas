using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABMMascotas.AccesoDatos
{
    public class DBHelper
    {
        private string cnnString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Veterinaria;Integrated Security=True";
        private SqlConnection cnn;
        private SqlCommand cmd;

        public DBHelper()
        {
            cnn = new SqlConnection(cnnString);
        }

        public DataTable Consultar(string query)
        {
            DataTable t = new DataTable();
            try { 
                cnn.Open();
                cmd = new SqlCommand(query, cnn);
                t.Load(cmd.ExecuteReader());
                cnn.Close();
            }
            catch (SqlException)
            {
                t = null;
            }

            return t;
        }

        public bool Guardar(Mascota m)
        {
            bool guardado = true;
            try
            {
                cnn.Open();
                cmd = new SqlCommand("INSERT INTO Mascotas VALUES(@codigo, @nombre, @especie, @sexo, @fecNac)", cnn);
                cmd.Parameters.AddWithValue("@codigo", m.Codigo);
                cmd.Parameters.AddWithValue("@nombre", m.Nombre);
                cmd.Parameters.AddWithValue("@especie", m.Especie);
                cmd.Parameters.AddWithValue("@sexo", m.Sexo);
                cmd.Parameters.AddWithValue("@fecNac", m.FechaNacimiento);
                guardado = cmd.ExecuteNonQuery() == 1;

                cnn.Close();

            }catch(Exception e)
            {
                guardado = false;
            }

            return guardado;
        }
    }
}
