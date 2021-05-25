using Session5.Modelo;
using Session5.ViewClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Session5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Saldo = 0;
            String booking = txtBuscar.Text;
            if (booking == String.Empty)
            {
                MessageBox.Show("Por favor, ingrese un valor en el campo booking reference");
                
                return;
            }
            using(Session5Entities modelo = new Session5Entities())
            {
                List<Vuelos> vuelos = (from s in modelo.Schedules
                                       join t in modelo.Tickets
                                       on s.ID equals t.ScheduleID
                                       where t.BookingReference == booking
                                       select new Vuelos {
                                           ID = s.ID,
                                           Name = s.FlightNumber+", "+s.Routes.Airports.IATACode+"-"+s.Routes.Airports1.IATACode+", "+s.Date.ToString()+", "+s.Time.ToString()

                                       }).ToList();
                vuelos.Insert(0, new Vuelos { ID = 0, Name = "Seleccione un vuelo" });
                comboBox1.DataSource = vuelos;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "ID";
            }
        }

        private void cargarInformacion()
        {
            Saldo = 0;
            
            panelServicio.Controls.Clear();
            int.TryParse(comboBox1.SelectedValue.ToString(), out int ID);
            if (ID == 0)
            {
                
                lblCabina.Text = lblPassport.Text= lblName.Text = "Seleccione un vuelo primero";
                return;
            }
            using(Session5Entities model = new Session5Entities())
            {
                String booking = txtBuscar.Text;
                Tickets t = model.Tickets.FirstOrDefault(x=> x.ScheduleID == ID && x.BookingReference ==booking);
                lblName.Text = $"{t.Firstname} {t.Lastname}";
                lblPassport.Text = t.PassportNumber;
                lblCabina.Text = t.CabinTypes.Name;
                llenarServicio(t.ID);

            }

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
               
                cargarInformacion();
            }
            catch (Exception)
            {

              
            }
        }
        public decimal Saldo { get; set; }


        public void llenarServicio(int ID)
        {
            using (Session5Entities modelo = new Session5Entities())
            {
                foreach (Amenities a in modelo.Amenities.ToList())
                {
                    
                    CheckBox c = new CheckBox()
                    {

                        Visible = true,
                        Text = $" {a.Service} ({(Math.Round(a.Price, 2).ToString())})",
                        Tag = a.ID,
                        Checked = false,
                        Enabled = true,
                        AutoSize = true
                    
                    };
                    c.CheckedChanged += (object sender, EventArgs e) =>
                    {
                        int.TryParse(c.Tag.ToString(), out int IDS);
                        using (Session5Entities model = new Session5Entities())
                        {
                            if (c.Checked && c.Enabled)
                            {
                                Saldo += model.Amenities.FirstOrDefault(x => x.ID == IDS).Price;
                            }
                            else if(c.Enabled)
                            {
                                Saldo -= model.Amenities.FirstOrDefault(x => x.ID == IDS).Price;

                            }
                            lblPrecio.Text = $"$ {Math.Round(Saldo, 2)}";
                            lblIVA.Text = $"$ {Math.Round(Saldo * (decimal)0.05, 2)}";
                            lblTotal.Text = $"$ {Math.Round(Saldo + Saldo * (decimal)0.05, 2) }";
                        }
                    };
                    if (modelo.AmenitiesTickets.Count(x => x.TicketID == ID && x.AmenityID == a.ID)!=0)
                    {
                        c.Checked = true;
                    }
                    int IDC = modelo.Tickets.FirstOrDefault(x => x.ID == ID).CabinTypeID;
                    if(modelo.CabinTypes.FirstOrDefault(x=>x.ID == IDC).Amenities.Count(x=>x.ID == a.ID) != 0)
                    {
                        c.Enabled = false;
                        c.Checked = true;
                        c.Text = $"{a.Service} (Free)";
                    }
                    panelServicio.Controls.Add(c);
                }
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int.TryParse(comboBox1.SelectedValue.ToString(), out int IDT);
            String booking = txtBuscar.Text;
           
            using (Session5Entities modelo  = new Session5Entities())
            {
                Tickets t = modelo.Tickets.FirstOrDefault(x => x.ScheduleID == IDT && x.BookingReference == booking);
                IDT = t.ID;
               modelo.AmenitiesTickets.RemoveRange(modelo.AmenitiesTickets.Where(X=>X.TicketID == IDT));
                modelo.SaveChanges();
                foreach (Control c in panelServicio.Controls)
                {
                    if (c.Enabled && (c as CheckBox).Checked )
                    {
                        int.TryParse(c.Tag.ToString(), out int IDA);
                        Amenities a = modelo.Amenities.FirstOrDefault(x => x.ID == IDA);
                        modelo.AmenitiesTickets.Add(new AmenitiesTickets
                        {
                            TicketID = IDT,
                            Price = a.Price,
                            AmenityID = a.ID,
                            Amenities = null,
                            Tickets = null
                        });
                    }
                  


                }
                int result = modelo.SaveChanges();
                MessageBox.Show($"Se guardaron {result} servicios");
            }
        }
    }
}
