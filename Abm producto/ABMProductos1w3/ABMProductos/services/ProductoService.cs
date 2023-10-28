using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Security.Cryptography;
using ABMProductos.data;
using System.Data.SqlClient;

namespace ABMProductos.services
{
    public class ProductoService
    {
        private AccesoDatos oAccesoDatos;

        public ProductoService()
        {
            oAccesoDatos = new AccesoDatos();   
        }
        
        public DataTable getMarcas()
        {
            return oAccesoDatos.ConsultarTabla("marcas");
        }

        public bool Save(Producto oProducto, bool esNuevo)
        {
            bool success = false;
            string sql;

            if (esNuevo)
                sql = "Insert into Productos Values(@codigo, @detalle,@tipo, @marca, @precio, @fecha)";
            else
                sql = "Update Productos SET detalle = @detalle, tipo = @tipo, marca = @marca,precio = @precio, fecha =  @fecha WHERE codigo = @codigo";

            //Convertimo un objeto Producto en una lista de parámetros
            List <Parametro> parametros = new List<Parametro>();
            parametros.Add(new Parametro("@codigo", oProducto.Codigo));
            parametros.Add(new Parametro("@detalle", oProducto.Detalle));
            parametros.Add(new Parametro("@marca", oProducto.Marca));
            parametros.Add(new Parametro("@tipo", oProducto.Tipo));
            parametros.Add(new Parametro("@precio", oProducto.Precio));
            parametros.Add(new Parametro("@fecha", oProducto.Fecha));
            
            success = oAccesoDatos.ActualizarConParametros(sql, parametros);

            return success;
        }

        
        public bool Delete(int codigo)
        {
            bool success = false;
            string sql = "DELETE from Productos WHERE codigo = @codigo";

            //Convertimo un objeto Producto en una lista de parámetros
            List<Parametro> parametros = new List<Parametro>();
            parametros.Add(new Parametro("@codigo", codigo));

            return oAccesoDatos.ActualizarConParametros(sql, parametros);
        }
       

        public List<Producto> getAll()
        {
            List<Producto> lst = new List<Producto>();

            DataTable t = oAccesoDatos.ConsultarTabla("productos");

            //for en datatable para convertir filas en objetos de negocio:

            Producto oProducto = null;
            foreach(DataRow fila in t.Rows)
            {
                //mapeamos
                oProducto = new Producto();
                int codigo = Convert.ToInt32(fila["codigo"]);
                oProducto.Codigo = codigo;
                oProducto.Detalle = fila["detalle"].ToString(); 
                int tipo = Convert.ToInt32(fila["tipo"]);
                oProducto.Tipo = tipo;
                int marca = Convert.ToInt32(fila["marca"]);
                oProducto.Marca = marca;
                double precio = Convert.ToDouble(fila["precio"]);
                oProducto.Precio = precio;
                DateTime fecha = Convert.ToDateTime(fila["fecha"]);
                oProducto.Fecha = fecha;
                lst.Add(oProducto);
            }
            return lst;
        }


    }
}
