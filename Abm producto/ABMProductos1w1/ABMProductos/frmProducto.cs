using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ABMProductos
{
    public partial class frmProducto : Form
    {
        bool nuevo;
        AccesoDatos oBD;
        List<Producto> lProductos;

        public frmProducto()
        {
            InitializeComponent();
            nuevo = false;
            oBD = new AccesoDatos();
            lProductos = new List<Producto>();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            Habilitar(false);
            CargarCombo(cboMarca, "Marcas");
            CargarLista(lstProducto, "Productos");
        }

        private void CargarLista(ListBox lista, string nombreTabla)
        {
            lProductos.Clear();
            //traer Productos de BD con DataTable
            DataTable tabla = oBD.ConsultarTabla(nombreTabla);
            foreach (DataRow fila in tabla.Rows)
            {
                Producto oProducto = new Producto();
                if (!fila.IsNull(0))
                    oProducto.Codigo = (int)fila[0];
                if (!fila.IsNull(1))
                    oProducto.Detalle = fila[1].ToString();
                if (!fila.IsNull(2))
                    oProducto.Tipo = Convert.ToInt32(fila[2]);
                if (!fila.IsNull(3))
                    oProducto.Marca = int.Parse(fila[3].ToString());
                if (!fila.IsNull(4))
                    oProducto.Precio = (double)fila["precio"];
                if (!fila.IsNull(5))
                    oProducto.Fecha = Convert.ToDateTime(fila["fecha"]);

                lProductos.Add(oProducto);
            }

            lista.Items.Clear();
            //lista.DataSource = lProductos;
            for (int i = 0; i < lProductos.Count; i++)
            {
                lista.Items.Add(lProductos[i].ToString());
            }
            lista.SelectedIndex=lista.Items.Count-1;

            ////traer Productos de BD con DataReader
            //oBD.LeerTabla(nombreTabla);
            //while (oBD.Lector.Read())
            //{
            //    //cargar Productos en lista de objetos
            //    Producto oProducto=new Producto();
            //    if (!oBD.Lector.IsDBNull(0))
            //        oProducto.Codigo = oBD.Lector.GetInt32(0);
            //    if (!oBD.Lector.IsDBNull(1))
            //        oProducto.Detalle = oBD.Lector.GetString(1);
            //    if (!oBD.Lector.IsDBNull(2))
            //        oProducto.Tipo = oBD.Lector.GetInt32(2);
            //    if (!oBD.Lector.IsDBNull(3))
            //        oProducto.Marca = oBD.Lector.GetInt32(3);
            //    if (!oBD.Lector.IsDBNull(4))
            //        oProducto.Precio = oBD.Lector.GetDouble(4);
            //    if (!oBD.Lector.IsDBNull(5))
            //        oProducto.Fecha = oBD.Lector.GetDateTime(5);
            //    lProductos.Add(oProducto);
            //}
            //oBD.Desconectar();

            //mostrar Productos en ListBox
            //lstProducto.Items.Clear();
            ////lstProducto.Items.AddRange(lProductos.ToArray());
            //foreach (Producto p in lProductos)
            //{
            //    lstProducto.Items.Add(p);
            //}
        }

        private void CargarCombo(ComboBox combo, string nombreTabla)
        {
            //DataTable tabla = oBD.ConsultarBD("SELECT * FROM " + nombreTabla);
            DataTable tabla = oBD.ConsultarTabla(nombreTabla);
            combo.DataSource = tabla;
            combo.ValueMember = tabla.Columns[0].ColumnName;    //"idMarca"
            combo.DisplayMember = tabla.Columns[1].ColumnName;  //"nombreMarca"
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void Habilitar(bool v)
        {
            //habilitar varios controles a la vez

        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            //validar datos
            //crear y cargar objeto
            Producto oProducto = new Producto();
            oProducto.Codigo = Convert.ToInt32(txtCodigo.Text);
            oProducto.Detalle = txtDetalle.Text;
            oProducto.Marca = Convert.ToInt32(cboMarca.SelectedValue);
            if (rbtNoteBook.Checked)
                oProducto.Tipo = 1;
            else
                oProducto.Tipo = 2;
            oProducto.Precio = Convert.ToDouble(txtPrecio.Text);
            oProducto.Fecha = dtpFecha.Value;
            
            string consulta = "";
            List<Parametro> lParametros = new List<Parametro>();

            if (nuevo)
            {
                //validar PK no exista
                if (!Existe(oProducto.Codigo))
                {
                    //insertar con SQL tradicional
                    //string consulta = "INSERT INTO Productos VALUES (" +
                    //                oProducto.Codigo + ",'" +
                    //                oProducto.Detalle + "'," +
                    //                oProducto.Tipo + "," +
                    //                oProducto.Marca + "," +
                    //                oProducto.Precio + ",'" +
                    //                oProducto.Fecha.ToString("yyyy-MM-dd") + "')";
                    //if (oBD.ActualizarBD(consulta) > 0)

                    //insertar con SQL con parametros
                    consulta = "INSERT INTO Productos VALUES (@codigo,@detalle,@tipo,@marca,@precio,@fecha)";
                   
                }
                else
                {
                    MessageBox.Show("El producto ya se enuentra registrado!!!");
                }
            }
            else
            {
                //update...
                 consulta = "UPDATE Productos SET detalle=@detalle,"
                                + "tipo=@tipo,"
                                + "marca=@marca,"
                                + "precio=@precio,"
                                + "fecha=@fecha"
                                + " WHERE codigo=@codigo";
            }
            lParametros.Clear();
            lParametros.Add(new Parametro("@codigo", oProducto.Codigo));
            lParametros.Add(new Parametro("@detalle", oProducto.Detalle));
            lParametros.Add(new Parametro("@tipo", oProducto.Tipo));
            lParametros.Add(new Parametro("@marca", oProducto.Marca));
            lParametros.Add(new Parametro("@precio", oProducto.Precio));
            lParametros.Add(new Parametro("@fecha", oProducto.Fecha));
            if (oBD.ActualizarBD(consulta, lParametros) > 0)
            {
                CargarLista(lstProducto, "Productos");
                MessageBox.Show("Se actualizó con éxito el Producto");
            }
            else
                MessageBox.Show("No se pudo actualizar el Producto");



        }

        private bool Existe(int pk)
        {
            bool encontro = false;
            foreach (Producto p in lProductos)
            {
                if (p.Codigo == pk)
                    encontro = true;
            }
            return encontro;
        }

        private void lstProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCampos(lProductos[lstProducto.SelectedIndex]);
        }

        private void CargarCampos(Producto p)
        {
            txtCodigo.Text = p.Codigo.ToString();
            txtDetalle.Text = p.Detalle;
            cboMarca.SelectedValue=p.Marca;
            if (p.Tipo == 1)
                rbtNoteBook.Checked = true;
            else
                rbtNetBook.Checked = true;
            txtPrecio.Text = p.Precio.ToString();
            dtpFecha.Value = p.Fecha;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Está seguro de eliminar el producto " + lProductos[lstProducto.SelectedIndex].Detalle+" ?"
                ,"ELIMINANDO"
                ,MessageBoxButtons.YesNo
                ,MessageBoxIcon.Error
                ,MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string consulta = "DELETE FROM Productos WHERE codigo=" + lProductos[lstProducto.SelectedIndex].Codigo;
                if (oBD.ActualizarBD(consulta)>0)
                {
                    CargarLista(lstProducto, "Productos");
                    MessageBox.Show("Se eliminó con éxito el Producto");
                }
                else
                    MessageBox.Show("No se pudo eliminar el Producto");

            }
        }
    }
}
