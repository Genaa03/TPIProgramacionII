﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAPI.Dominio;
using DataAPI.Servicios.Implementaciones;
using FrontUTN.Http;
using FrontUTN.Presentaciones;
using Newtonsoft.Json;

namespace FrontUTN.Presentaciones
{
    public partial class FormInscripcionAlumno : Form
    {
        private static FormInscripcionAlumno instancia;
        private Alumno alumno;
        private GestorAPI gestor;
        public FormInscripcionAlumno()
        {
            InitializeComponent();
            alumno = new Alumno();
            gestor = new GestorAPI();
        }

        public static FormInscripcionAlumno ObtenerInstancia()
        {
            if (instancia == null)
            {
                instancia = new FormInscripcionAlumno();
            }
            return instancia;
        }

        private async void FormAltaAlumno_Load(object sender, EventArgs e)
        {
            await CargarBarriosAsync();
            await CargarEstadosCivilAsync();
            await CargarSituacionHabAsync();
            await CargarSituacionLabAsync();
            await CargarTecnicaturasAsync();
            await CargarTiposDNIAsync();
            ProximoAlumno();
        }

        private async Task CargarBarriosAsync()
        {
            string url = "http://localhost:5041/barrios";
            var result = await ClientSingleton.GetInstance().GetAsync(url);
            var lst = JsonConvert.DeserializeObject<List<Barrio>>(result);
            cboBarrio.DataSource = lst;
            cboBarrio.DisplayMember = "barrio";
            cboBarrio.ValueMember = "id";
            cboBarrio.SelectedIndex = -1;    

        }

        private async Task CargarTiposDNIAsync()
        {
            string url = "http://localhost:5041/tiposDNI";
            var result = await ClientSingleton.GetInstance().GetAsync(url);
            var lst = JsonConvert.DeserializeObject<List<TipoDNI>>(result);
            cboTipoDni.DataSource = lst;
            cboTipoDni.DisplayMember = "tipo_dni";
            cboTipoDni.ValueMember = "id";
            cboTipoDni.SelectedIndex = -1;

        }

        private async Task CargarTecnicaturasAsync()
        {
            string url = "http://localhost:5041/tecnicaturas";
            var result = await ClientSingleton.GetInstance().GetAsync(url);
            var lst = JsonConvert.DeserializeObject<List<Tecnicatura>>(result);
            cboTecnicatura.DataSource = lst;
            cboTecnicatura.DisplayMember = "tecnicatura";
            cboTecnicatura.ValueMember = "id";
            cboTecnicatura.SelectedIndex = -1;

        }

        private async Task CargarSituacionHabAsync()
        {
            string url = "http://localhost:5041/situacionHab";
            var result = await ClientSingleton.GetInstance().GetAsync(url);
            var lst = JsonConvert.DeserializeObject<List<SituacionHab>>(result);
            cboSituacionHabitacional.DataSource = lst;
            cboSituacionHabitacional.DisplayMember = "situacion_hab";
            cboSituacionHabitacional.ValueMember = "id";
            cboSituacionHabitacional.SelectedIndex = -1;

        }

        private async Task CargarSituacionLabAsync()
        {
            string url = "http://localhost:5041/situacionLab";
            var result = await ClientSingleton.GetInstance().GetAsync(url);
            var lst = JsonConvert.DeserializeObject<List<SituacionLab>>(result);
            cboSituacionLaboral.DataSource = lst;
            cboSituacionLaboral.DisplayMember = "situacion_lab";
            cboSituacionLaboral.ValueMember = "id";
            cboSituacionLaboral.SelectedIndex = -1;

        }

        private async Task CargarEstadosCivilAsync()
        {
            string url = "http://localhost:5041/estadosCivil";
            var result = await ClientSingleton.GetInstance().GetAsync(url);
            var lst = JsonConvert.DeserializeObject<List<EstadoCivil>>(result);
            cboEstadoCivil.DataSource = lst;
            cboEstadoCivil.DisplayMember = "estado_civil";
            cboEstadoCivil.ValueMember = "id";
            cboEstadoCivil.SelectedIndex = -1;

        }

        private void ProximoAlumno()
        {
            lblNroAlumno.Text = "Alumno N°: " + gestor.GetProximoAlumno().ToString();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        private void limpiar()
        {
            txtApellido.Text = "";
            txtNombre.Text = "";
            txtDni.Text = "";
            txtDireccion.Text = "";
            dtpFechaNacimiento.Value = DateTime.Today;
            cboBarrio.SelectedIndex = -1;
            cboEstadoCivil.SelectedIndex = -1;
            cboSituacionHabitacional.SelectedIndex = -1;
            cboSituacionLaboral.SelectedIndex = -1;
            cboTecnicatura.SelectedIndex = -1;
            cboTipoDni.SelectedIndex = -1;
            ProximoAlumno();
        }

        private bool validar()
        {
            if(String.IsNullOrEmpty(txtApellido.Text) || String.IsNullOrEmpty(txtNombre.Text) || String.IsNullOrEmpty(txtDireccion.Text) 
                || String.IsNullOrEmpty(txtDni.Text) || cboBarrio.SelectedIndex == -1 || cboEstadoCivil.SelectedIndex == -1 
                || cboSituacionHabitacional.SelectedIndex == -1 || cboSituacionLaboral.SelectedIndex == -1 || cboTecnicatura.SelectedIndex == -1 
                || cboTipoDni.SelectedIndex == -1)
            {
                MessageBox.Show("ERROR. Algun campo se encuentra vacio.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (Int32.TryParse(txtDni.Text, out int a) == false)
            {
                MessageBox.Show("ERROR. Ingrese solo numeros en el N° de DNI.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (txtDni.Text.Length > 10)
            {
                MessageBox.Show("ERROR. El N° de DNI solo puede tener 10 digitos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if(DateTime.Today.Year - dtpFechaNacimiento.Value.Year < 17)
            {
                MessageBox.Show("ERROR. El alumno debe ser mayor a 17 años.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private async Task<bool> InscribirAlumno()
        {
            //Datos del Alumno
            if (validar())
            {
                alumno.id = Convert.ToInt32(gestor.GetProximoAlumno());
                alumno.nombre = txtNombre.Text;
                alumno.apellido = txtApellido.Text;
                alumno.tipo_dni = (int)cboTipoDni.SelectedValue;
                alumno.nro_dni = txtDni.Text;
                alumno.tecnicatura = (int)cboTecnicatura.SelectedValue;
                alumno.fecha_nac = dtpFechaNacimiento.Value;
                alumno.estado_civil = (int)cboEstadoCivil.SelectedValue;
                alumno.situacion_habitacional = (int)cboSituacionHabitacional.SelectedValue;
                alumno.situacion_laboral = (int)cboSituacionLaboral.SelectedValue;
                alumno.barrio = (int)cboEstadoCivil.SelectedValue;
                alumno.direccion = txtDireccion.Text;

                string bodyContent = JsonConvert.SerializeObject(alumno);

                string url = "http://localhost:5041/inscripcionAlumno";
                var result = await ClientSingleton.GetInstance().PostAsync(url, bodyContent);

                if (result.Equals("true"))
                {
                    MessageBox.Show("Alumno inscripto.", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    limpiar();
                    return true;
                }
                else
                {
                    MessageBox.Show("ERROR. No se pudo inscribir el alumno", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("¿Seguro desea cancelar la inscripcion?","ATENCION",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.Close();
                VentanaPrincipal.ObtenerInstancia().Show();
            }
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            await InscribirAlumno();
        }

    }
}
