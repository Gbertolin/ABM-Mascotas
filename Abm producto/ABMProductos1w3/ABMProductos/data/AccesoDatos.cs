using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using ABMProductos.data;

namespace ABMProductos
{
    public class AccesoDatos
    {
        private string cadenaConexion = @"Data Source=.\SQLEXPRESS;Initial Catalog=informatica;Integrated Security=True";
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

        public void LeerTabla(string nombreTabla)
        {
            Conectar();
            comando.CommandText = "SELECT * FROM " + nombreTabla + " ORDER BY 2";
            lector = comando.ExecuteReader();
        }

        public DataTable ConsultarTabla(string nombreTabla)
        {
            DataTable tabla = new DataTable();
            Conectar();
            comando.CommandText = "SELECT * FROM " + nombreTabla + " ORDER BY 2";
            tabla.Load(comando.ExecuteReader());
            Desconectar();
            return tabla;
        }
        public DataTable ConsultarBD(string consultaSQL)
        {
            DataTable tabla = new DataTable();
            Conectar();
            comando.CommandText = consultaSQL;
            tabla.Load(comando.ExecuteReader());
            Desconectar();
            return tabla;
        }

        public void Desconectar()
        {
            conexion.Close();
        }

        private void Conectar()
        {
            conexion.Open();
            comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.Text;
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

        public bool ActualizarConParametros(string sql, List<Parametro> parametros)
        {
            int filasAfectadas = 0;
            Conectar();
            comando.CommandText = sql;
            foreach (Parametro param in parametros)
            {
                comando.Parameters.AddWithValue(param.Clave, param.Valor);
            }
            filasAfectadas = comando.ExecuteNonQuery();
            Desconectar();
            return filasAfectadas == 1;
        }
    }
}
