using ABMMascotas.AccesoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//CURSO – LEGAJO – APELLIDO – NOMBRE

namespace ABMMascotas
{
    public partial class frmMascota : Form
    {
        private DBHelper helper;

        public frmMascota()
        {
            InitializeComponent();
            helper = new DBHelper();
        }

        private void frmMascota_Load(object sender, EventArgs e)
        {
            //cargar Combo:
            DataTable t = helper.Consultar("SELECT * FROM Especies");
            cboEspecie.DataSource = t;
            cboEspecie.DisplayMember = t.Columns[1].ColumnName;
            cboEspecie.ValueMember = t.Columns[0].ColumnName;

            CargarLista();

        }


        // Consultar los datos de las mascotas
        // y llenar el modelo de datos del lstMascotas con objetos Mascotas:
        private void CargarLista()
        {
            DataTable t = helper.Consultar("SELECT * FROM Mascotas");

            lstMascotas.Items.Clear();
            foreach (DataRow row in t.Rows)
            {
                Mascota m = new Mascota();
                m.Codigo = Convert.ToInt32(row["codigo"]);
                m.Nombre = row["nombre"].ToString();
                m.Especie = Convert.ToInt32(row["especie"]);
                m.Sexo = Convert.ToInt32(row["sexo"]);
                m.FechaNacimiento = Convert.ToDateTime(row["fechaNacimiento"]);

                lstMascotas.Items.Add(m);

            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (btnSalir.Text == "Salir")
                Close();
            else
            {
                LimpiarCampos();
                btnNuevo.Enabled = true;
                btnGrabar.Enabled = false;
                btnSalir.Text = "Salir";
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            btnNuevo.Enabled = false;
            btnGrabar.Enabled = true;
            btnSalir.Text = "Cancelar";

        }

        private void LimpiarCampos()
        {
            txtCodigo.Text = string.Empty;
            txtNombre.Text = string.Empty;
            cboEspecie.SelectedIndex = 0;
            rbtHembra.Checked = false;
            rbtMacho.Checked = false;
            dtpFechaNacimiento.Value = DateTime.Now;
            txtCodigo.Focus();

        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            //valida datos...
            if (ValidarDatos())
            {
                int codigo = int.Parse(txtCodigo.Text);
                if (!Existe(codigo))
                {
                    //Intentar grabar el registro:
                    Mascota m = new Mascota();
                    m.Codigo = Convert.ToInt32(txtCodigo.Text);
                    m.Nombre = txtNombre.Text;
                    m.Especie = (int)cboEspecie.SelectedValue;
                    if (rbtHembra.Checked)
                        m.Sexo = 2;
                    else
                        m.Sexo = 1;
                    m.FechaNacimiento = dtpFechaNacimiento.Value;

                    if (helper.Guardar(m))
                    {
                        MessageBox.Show("Se registró con éxito!", "Confirmación");
                        LimpiarCampos();
                        btnNuevo.Enabled = true;
                        btnGrabar.Enabled = false;
                        btnSalir.Text = "Salir";
                        CargarLista();
                    }
                    else
                    {
                        MessageBox.Show("Ha ocurrido un error, intente nuevamente!", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Mascota ya cargada!", "Validaciones");
                }
            }
            else
            {
                MessageBox.Show("Debe completar todos los campos!", "Validaciones");
            }


            //crear objeto

            //insert usando parámetros
        }

        private bool Existe(int codigo)
        {
            for (int i = 0; i < lstMascotas.Items.Count; i++)
            {
                Mascota m = (Mascota)lstMascotas.Items[i];
                if (m.Codigo == codigo)
                    return true;
            }

            return false;
        }

        private bool ValidarDatos()
        {
            bool v = true;
            if (string.IsNullOrEmpty(txtCodigo.Text))
                v = false;

            if (string.IsNullOrEmpty(txtNombre.Text))
                v = false;
            if (cboEspecie.SelectedIndex == -1)
                v = false;

            if (!rbtHembra.Checked && !rbtMacho.Checked)
                v = false;


            return v;
        }

        private void lstMascotas_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }

        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
