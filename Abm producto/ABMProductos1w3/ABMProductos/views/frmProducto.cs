using ABMProductos.services;
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
        //objeto instancia de la Clase delegada para manejo de la BD
        List<Producto> listaProductos;  //lista dinamica para n objetos Producto
        private ProductoService service;
        private bool esNuevo = false;
        public frmProducto()
        {
            InitializeComponent();

            listaProductos = new List<Producto>();
            service = new ProductoService();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            CargarCombo(cboMarca, "marcas");
            CargarLista(lstProducto, "productos");
            Habilitar(true);
        }

        private void Habilitar(bool v)
        {
            gbProducto.Enabled = !v;
            lstProducto.Enabled = v;
            btnNuevo.Enabled = v;
            btnSalir.Enabled = v;
            btnEditar.Enabled = !v;
            btnGrabar.Enabled = !v;
            btnBorrar.Enabled = !v;
            btnCancelar.Enabled = !v;
        }

        private void CargarLista(ListBox lista, string nombreTabla) //usando DataReader conectado
        {
            listaProductos.Clear();
            //traer  datos de BD
            lista.Items.Clear();
            listaProductos = service.getAll();
            //lista.DataSource= listaProductos;     

            lista.Items.AddRange(listaProductos.ToArray());
            /*for (int i = 0; i < listaProductos.Count; i++)
            {
                lista.Items.Add(listaProductos[i]);
            }*/
        }

        private void CargarCombo(ComboBox combo, string nombreTabla)
        {
            // DataTable tabla = oBD.ConsultarBD("SELECT * FROM " + nombreTabla + " ORDER BY 2");
            DataTable tabla = service.getMarcas();//oBD.ConsultarTabla(nombreTabla);
            combo.DataSource = tabla;
            combo.ValueMember = tabla.Columns[0].ColumnName;    //"idMarca"
            combo.DisplayMember = tabla.Columns[1].ColumnName;  //"nombreMarca"
            combo.DropDownStyle = ComboBoxStyle.DropDownList;   //no permite edición, solo selección
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            //validar datos
            if (ValidarDatos())
            {
                //crear objeto
                Producto oProducto = new Producto();
                oProducto.Codigo = Convert.ToInt32(txtCodigo.Text);
                oProducto.Detalle = txtDetalle.Text;
                oProducto.Marca = (int)cboMarca.SelectedValue;
                if (rbtNoteBook.Checked)
                    oProducto.Tipo = 1;
                else
                    oProducto.Tipo = 2;
                oProducto.Precio = Convert.ToDouble(txtPrecio.Text);
                oProducto.Fecha = dtpFecha.Value;

                if (esNuevo)
                {
                    if (Existe(oProducto.Codigo))
                    {
                        MessageBox.Show("Producto ya cargado!", "Validación");
                        return;
                    }
                }

                //validar no exista PK si no es identity
                bool ok = service.Save(oProducto, esNuevo);
                //validar e informar si se pudo actualizar con exito!
                //....
                Habilitar(true);
                LimpiarCampos();
                CargarLista(lstProducto, "productos");
            }
        }

        private bool Existe(int pk)
        {
            bool encontro = false;
            //foreach (Producto p in listaProductos)
            //{
            //    if (p.Codigo == pk)
            //        encontro = true;
            //}
            for (int i = 0; i < listaProductos.Count; i++)
            {
                if (listaProductos[i].Codigo == pk)
                {
                    encontro = true;
                    break;
                }
            }
            return encontro;
        }

        private bool ValidarDatos()
        {
            if (txtCodigo.Text == string.Empty)
            {
                MessageBox.Show("Debe ingresar un codigo...");
                txtCodigo.Focus();
                return false;
            }
            if (txtDetalle.Text == string.Empty)
            {
                MessageBox.Show("Debe ingresar un detalle...");
                txtDetalle.Focus();
                return false;
            }
            if (cboMarca.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una marca...");
                cboMarca.Focus();
                return false;
            }
            // validar campos vacios 
            try
            {
                double precio = Double.Parse(txtPrecio.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Debe ingresar una cantidad numerica para el precio...");
                txtPrecio.Focus();
                return false;
            }
            /* if(dtpFecha.Value <= DateTime.Now)
             {

             }*/
            return true;
        }

        private void lstProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = lstProducto.SelectedIndex;
            if (selected != -1)
            {
                Producto p = listaProductos[selected];
                txtCodigo.Text = p.Codigo.ToString();
                txtDetalle.Text = p.Detalle;
                cboMarca.SelectedValue = p.Marca;
                if (p.Tipo == 1)
                {
                    rbtNoteBook.Checked = true;
                }
                else
                {
                    rbtNetBook.Checked = true;
                }

                txtPrecio.Text = p.Precio.ToString();
                dtpFecha.Value = p.Fecha;
                btnEditar.Enabled = true;
                btnBorrar.Enabled = true;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Habilitar(false);
            esNuevo = true;
            btnBorrar.Enabled = false;
            btnEditar.Enabled = false;
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            foreach (Control control in this.gbProducto.Controls)
            {
                if (control is ComboBox)
                {
                    ((ComboBox)control).SelectedIndex = -1;
                }
                if (control is RadioButton)
                {
                    ((RadioButton)control).Checked = false;
                }

                if (control is TextBox)
                {
                    control.Text = String.Empty;
                }

                if (control is DateTimePicker)
                {
                    ((DateTimePicker)control).Value = DateTime.Now;
                }
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Habilitar(false);
            esNuevo = false;
            btnBorrar.Enabled = false;
            btnEditar.Enabled = false;


        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            int index = lstProducto.SelectedIndex;
            if (MessageBox.Show("Seguro que desea eliminar el producto" +
                " seleccionado?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (service.Delete(listaProductos[index].Codigo))
                {
                    MessageBox.Show("Producto eliminado!", "Info");
                    Habilitar(true);
                    LimpiarCampos();
                    CargarLista(lstProducto, "Productos");
                }
                else
                {
                    MessageBox.Show("No se pudo borrar el producto seleccionado!", "Error");
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seguro que desea salir?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}

