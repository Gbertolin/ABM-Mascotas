using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ABMProductos
{
    public class AccesoDatos
    {
        private string cadenaConexion = @"Data Source=.\SQLEXPRESS;Initial Catalog=Informatica;Integrated Security=True";
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        public SqlDataReader Lector
        {
            get { return lector; }
            set { lector = value; }
        }
        public AccesoDatos()
        {
            conexion = new SqlConnection(cadenaConexion);
        }

        private void Conectar()
        {
            conexion.Open();
            comando=new SqlCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.Text;
        }
        public void Desconectar()
        {
            conexion.Close();
        }
        public void LeerTabla(string nombreTabla) //metodo que carga un datareader
        {
            Conectar();
            comando.CommandText = "SELECT * FROM " + nombreTabla;
            lector = comando.ExecuteReader();
        }
        
        public DataTable ConsultarTabla(string nombreTabla) //metodo que devuelve datatable de una tabla
        {
            DataTable tabla = new DataTable();
            Conectar();
            comando.CommandText = "SELECT * FROM " + nombreTabla;
            tabla.Load(comando.ExecuteReader());
            Desconectar();
            return tabla;
        }
        public DataTable ConsultarBD(string consultaSQL)    //metodo que devuelve datatable de cualquier consulta
        {
            DataTable tabla = new DataTable();
            Conectar();
            comando.CommandText = consultaSQL;
            tabla.Load(comando.ExecuteReader());
            Desconectar();
            return tabla;
        }
        public int ActualizarBD(string consultaSQL)
        {
            int filasAfectadas = 0;
            Conectar();
            comando.CommandText = consultaSQL;
            filasAfectadas = comando.ExecuteNonQuery();
            Desconectar();
            return filasAfectadas;
        }
        public int ActualizarBD(string consultaSQL,List<Parametro> lParametros)
        {
            int filasAfectadas = 0;
            Conectar();
            comando.CommandText = consultaSQL;

            foreach (Parametro p in lParametros)
            {
                comando.Parameters.AddWithValue(p.Nombre, p.Valor);
            }

            filasAfectadas = comando.ExecuteNonQuery();
            Desconectar();
            return filasAfectadas;
        }
    }
}
