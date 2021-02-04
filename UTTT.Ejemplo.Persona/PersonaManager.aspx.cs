#region Using

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web.UI.WebControls;
using UTTT.Ejemplo.Linq.Data.Entity;
using UTTT.Ejemplo.Persona.Control;
using UTTT.Ejemplo.Persona.Control.Ctrl;

#endregion

namespace UTTT.Ejemplo.Persona
{
    public partial class PersonaManager : System.Web.UI.Page
    {
        #region Variables

        private SessionManager session = new SessionManager();
        private int idPersona = 0;
        private UTTT.Ejemplo.Linq.Data.Entity.Persona baseEntity;
        private DataContext dcGlobal = new DcGeneralDataContext();
        private int tipoAccion = 0;

        #endregion

        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Response.Buffer = true;
                this.session = (SessionManager)this.Session["SessionManager"];
                this.idPersona = this.session.Parametros["idPersona"] != null ?
                    int.Parse(this.session.Parametros["idPersona"].ToString()) : 0;
                if (this.idPersona == 0)
                {
                    this.baseEntity = new Linq.Data.Entity.Persona();
                    this.tipoAccion = 1;
                }
                else
                {
                    this.baseEntity = dcGlobal.GetTable<Linq.Data.Entity.Persona>().First(c => c.id == this.idPersona);
                    this.tipoAccion = 2;
                }

                if (!this.IsPostBack)
                {
                    if (this.session.Parametros["baseEntity"] == null)
                    {
                        this.session.Parametros.Add("baseEntity", this.baseEntity);
                    }
                    List<CatSexo> lista = dcGlobal.GetTable<CatSexo>().ToList();
                    CatSexo catTemp = new CatSexo();
                    catTemp.id = -1;
                    catTemp.strValor = "Seleccionar";
                    lista.Insert(0, catTemp);
                    this.ddlSexo.DataTextField = "strValor";
                    this.ddlSexo.DataValueField = "id";
                    this.ddlSexo.DataSource = lista;
                    this.ddlSexo.DataBind();

                    this.ddlSexo.SelectedIndexChanged += new EventHandler(ddlSexo_SelectedIndexChanged);
                    this.ddlSexo.AutoPostBack = true;
                    if (this.idPersona == 0)
                    {
                        this.lblAccion.Text = "Agregar";
                    }
                    else
                    {
                        this.lblAccion.Text = "Editar";
                        this.txtNombre.Text = this.baseEntity.strNombre;
                        this.txtAPaterno.Text = this.baseEntity.strAPaterno;
                        this.txtAMaterno.Text = this.baseEntity.strAMaterno;
                        this.txtClaveUnica.Text = this.baseEntity.strClaveUnica;
                        DateTime? fecha = this.baseEntity.dteFechaNacimiento;
                        this.txtHiden.Value = fecha.ToString();
                        this.dteCalendar.TodaysDate = (DateTime)fecha;
                        this.dteCalendar.SelectedDate = (DateTime)fecha;
                        this.txtCodigoPostal.Text = this.baseEntity.strCodigoPostal;
                        this.txtRFC.Text = this.baseEntity.strRFC;
                        this.txtCorreoElectronico.Text = this.baseEntity.strCorreoElectronico;

                        this.setItem(ref this.ddlSexo, baseEntity.CatSexo.strValor);
                    }
                }

            }
            catch (Exception _e)
            {
                this.showMessage("se Genero un problema al cargar");
                this.Response.Redirect("~/PersonaPrincipal.aspx", false);
            }

        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {

                DataContext dcGuardar = new DcGeneralDataContext();
                UTTT.Ejemplo.Linq.Data.Entity.Persona persona = new Linq.Data.Entity.Persona();
                if (this.idPersona == 0)
                {
                    persona.strClaveUnica = this.txtClaveUnica.Text.Trim();
                    persona.strNombre = this.txtNombre.Text.Trim();
                    persona.strAMaterno = this.txtAMaterno.Text.Trim();
                    persona.strAPaterno = this.txtAPaterno.Text.Trim();
                    persona.idCatSexo = int.Parse(this.ddlSexo.Text);
                    DateTime fechaNacimiento = this.dteCalendar.SelectedDate.Date;
                    persona.dteFechaNacimiento = fechaNacimiento;
                    persona.strCorreoElectronico = this.txtCorreoElectronico.Text.Trim();
                    persona.strCodigoPostal = this.txtCodigoPostal.Text.Trim();
                    persona.strRFC = this.txtRFC.Text.Trim();

                    String mensaje = String.Empty;
                    //Validacion de datos correctos


                    if (!this.validacion(persona, ref mensaje))
                    {
                        this.lblError.Text = mensaje;
                        this.lblError.Visible = true;
                        return;
                    }


                    if (!this.validaSql(ref mensaje))
                    {
                        this.lblError.Text = mensaje;
                        this.lblError.Visible = true;
                        return;
                    }

                    if (!this.validaHTML(ref mensaje))
                    {
                        this.lblError.Text = mensaje;
                        this.lblError.Visible = true;
                        return;
                    }
                    dcGuardar.GetTable<UTTT.Ejemplo.Linq.Data.Entity.Persona>().InsertOnSubmit(persona);
                    dcGuardar.SubmitChanges();
                    this.showMessage("Se agrego correctamente.");
                    this.Response.Redirect("~/PersonaPrincipal.aspx", false);

                }
                if (this.idPersona > 0)
                {
                    persona = dcGuardar.GetTable<UTTT.Ejemplo.Linq.Data.Entity.Persona>().First(c => c.id == idPersona);
                    persona.strClaveUnica = this.txtClaveUnica.Text.Trim();
                    persona.strNombre = this.txtNombre.Text.Trim();
                    persona.strAMaterno = this.txtAMaterno.Text.Trim();
                    persona.strAPaterno = this.txtAPaterno.Text.Trim();
                    persona.idCatSexo = int.Parse(this.ddlSexo.Text);
                    DateTime fechaNacimiento = this.dteCalendar.SelectedDate.Date;
                    persona.dteFechaNacimiento = fechaNacimiento;
                    persona.strCorreoElectronico = this.txtCorreoElectronico.Text.Trim();
                    persona.strCodigoPostal = this.txtCodigoPostal.Text.Trim();
                    persona.strRFC = this.txtRFC.Text.Trim();
                    String mensaje = String.Empty;
                    if (!this.validacion(persona, ref mensaje))
                    {
                        this.lblError.Text = mensaje;
                        this.lblError.Visible = true;
                        return;
                    }
                    if (!this.validaSql(ref mensaje))
                    {
                        this.lblError.Text = mensaje;
                        this.lblError.Visible = true;
                        return;
                    }
                    if (!this.validaHTML(ref mensaje))
                    {
                        this.lblError.Text = mensaje;
                        this.lblError.Visible = true;
                        return;
                    }
                    dcGuardar.SubmitChanges();
                    this.showMessage("Se edito correctamente.");
                    this.Response.Redirect("~/PersonaPrincipal.aspx", false);
                }
            }
            catch (Exception _e)
            {
                // this.showMessageException(_e.Message);

                var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                string strHost = smtpSection.Network.Host;
                int port = smtpSection.Network.Port;
                string strUserName = smtpSection.Network.UserName;
                string strFromPass = smtpSection.Network.Password;
                SmtpClient smtp = new SmtpClient(strHost, port);
                MailMessage msg = new MailMessage();
                string body = "<h2>Se ha detectado un error: " + _e.Message + "</h2>";
                msg.From = new MailAddress(smtpSection.From, "CORREO DE ERRORES");
                msg.To.Add(new MailAddress("18300988@uttt.edu.mx"));
                msg.Subject = "Nuevo error"; ;
                msg.IsBodyHtml = true;
                msg.Body = body;
                smtp.Credentials = new NetworkCredential(strUserName, strFromPass);
                smtp.EnableSsl = true;
                smtp.Send(msg);
                Response.Redirect("~/ErrorPage.aspx", false);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Response.Redirect("~/PersonaPrincipal.aspx", false);
            }
            catch (Exception _e)
            {
                this.showMessage("Error");
            }
        }

        protected void ddlSexo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int idSexo = int.Parse(this.ddlSexo.Text);
                Expression<Func<CatSexo, bool>> predicateSexo = c => c.id == idSexo;
                predicateSexo.Compile();
                List<CatSexo> lista = dcGlobal.GetTable<CatSexo>().Where(predicateSexo).ToList();
                CatSexo catTemp = new CatSexo();
                this.ddlSexo.DataTextField = "strValor";
                this.ddlSexo.DataValueField = "id";
                this.ddlSexo.DataSource = lista;
                
            }
            catch (Exception)
            {
                this.showMessage("Error");
            }
        }

        #endregion

        #region Metodos

        public void setItem(ref DropDownList _control, String _value)
        {
            foreach (ListItem item in _control.Items)
            {
                if (item.Value == _value)
                {
                    item.Selected = true;
                    break;
                }
            }
            _control.Items.FindByText(_value).Selected = true;
        }

        #endregion

        public bool validacion(UTTT.Ejemplo.Linq.Data.Entity.Persona _persona, ref String _mensaje)
        {

            if (_persona.idCatSexo == -1)
            {
                _mensaje = "Seleccione sexo Femenino o Masculino ";
                return false;
            }
            int i = 0;
            if (int.TryParse(_persona.strClaveUnica, out i) == false)
            {
                _mensaje = "no es un numero";
                return false;
            }
            if (_persona.strClaveUnica.Equals(String.Empty))
            {
                _mensaje = "Esta vacio";
                return false;
            }
            if (int.Parse(_persona.strClaveUnica) < 100 || int.Parse(_persona.strClaveUnica) > 999)
            {
                _mensaje = "Clave de 3 num";
                return false;
            }

            if (_persona.strNombre.Equals(String.Empty))
            {
                _mensaje = "Esta vacio";
                return false;
            }
            if (_persona.strNombre.Length > 50)
            {
                _mensaje = "Se ha revasado los caracteres contemplados";
                return false;
            }

            if (_persona.strAPaterno.Equals(String.Empty))
            {
                _mensaje = "Esta vacio";
                return false;
            }
            if (_persona.strAPaterno.Length > 50)
            {
                _mensaje = "Se ha revasado los caracteres contemplados";
                return false;
            }
            if (_persona.strAMaterno.Equals(String.Empty))
            {
                _mensaje = "Esta vacio";
                return false;
            }
            if (_persona.strAPaterno.Length > 50)
            {
                _mensaje = "Se ha revasado los caracteres contemplados";
                return false;
            }
            DateTime? fecha = this.baseEntity.dteFechaNacimiento;
            this.txtHiden.Value = fecha.ToString();
            TimeSpan timeSpan = DateTime.Now - fecha.Value.Date;
            if (timeSpan.Days < 6570)
            {
                _mensaje = "No tiene edad suficiente";
                return false;
            }




            if (_persona.strCorreoElectronico.Equals(String.Empty))
            {
                _mensaje = "Esta vacio";
                return false;
            }
            if (_persona.strCorreoElectronico.Length > 50)
            {
                _mensaje = "Se ha revasado los caracteres contemplados";
                return false;
            }
            if (_persona.strRFC.Equals(String.Empty))
            {
                _mensaje = " Vacio";
                return false;
            }
            if (_persona.strRFC.Length > 50)
            {
                _mensaje = "Se ha revasado los caracteres contemplados";
                return false;
            }
            if (int.TryParse(_persona.strCodigoPostal, out i) == false)
            {
                _mensaje = "No es un  numero";
                return false;
            }
            if (_persona.strCodigoPostal.Equals(String.Empty))
            {
                _mensaje = "Esta vacio";
                return false;
            }

            return true;
        }

        private bool validaSql(ref String _mensaje)
        {
            CtrlValidaInyeccion valida = new CtrlValidaInyeccion();
            string mensajeFuncion = string.Empty;
            if (valida.sqlInyectionValida(this.txtNombre.Text.Trim(), ref mensajeFuncion, "Nombre", ref this.txtNombre))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.sqlInyectionValida(this.txtAPaterno.Text.Trim(), ref mensajeFuncion, "A Paterno", ref this.txtAPaterno))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.sqlInyectionValida(this.txtAMaterno.Text.Trim(), ref mensajeFuncion, "A Materno", ref this.txtAMaterno))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.sqlInyectionValida(this.txtCorreoElectronico.Text.Trim(), ref mensajeFuncion, "Correo Electronico", ref this.txtCorreoElectronico))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.sqlInyectionValida(this.txtRFC.Text.Trim(), ref mensajeFuncion, "RFC", ref this.txtRFC))
            {
                _mensaje = mensajeFuncion;
                return false;
            }

            return true;
        }

        private bool validaHTML(ref String _mensaje)
        {
            CtrlValidaInyeccion valida = new CtrlValidaInyeccion();
            string mensajeFuncion = string.Empty;
            if (valida.htmlInyectionValida(this.txtNombre.Text.Trim(), ref mensajeFuncion, "Nombre", ref this.txtNombre))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.htmlInyectionValida(this.txtAPaterno.Text.Trim(), ref mensajeFuncion, "A Paterno", ref this.txtAPaterno))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.htmlInyectionValida(this.txtAMaterno.Text.Trim(), ref mensajeFuncion, "A Materno", ref this.txtAMaterno))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.htmlInyectionValida(this.txtCorreoElectronico.Text.Trim(), ref mensajeFuncion, "Correo Electronico", ref this.txtCorreoElectronico))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.htmlInyectionValida(this.txtRFC.Text.Trim(), ref mensajeFuncion, "RFC", ref this.txtRFC))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
           

            return true;
        }


        protected void dteCalendar_SelectionChanged(object sender, EventArgs e)
        {
            txtHiden.Value = dteCalendar.SelectedDate.ToString();
        }
    }
}