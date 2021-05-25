using Session5.Modelo;
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
    public partial class Sumary : Form
    {
        public Sumary()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            using (Session5Entities Modelo = new Session5.Modelo.Session5Entities())
            {
                Schedules schedules = Modelo.Schedules.FirstOrDefault(x =>  x.ID.ToString() == txtBuscar.Text);
                dataGridView1.Columns.Add("Tipo Cabina", "Tipo Cabina");
                foreach (CabinTypes c in Modelo.CabinTypes.ToList())
                {
                    dataGridView1.Rows.Add(c.Name);
                }

                foreach (Amenities a in Modelo.Amenities.ToList())
                {
                    dataGridView1.Columns.Add(a.Service, a.Service);
                }

                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    foreach (DataGridViewColumn c in dataGridView1.Columns)
                    {

                        if (c.Index != 0)
                        {

                         
                            int cantidad = 0;
                           
                            foreach (Tickets t in schedules.Tickets.Where(x => x.CabinTypes.Name == r.Cells[0].Value.ToString()).ToList())
                            {

                                
                                cantidad += t.AmenitiesTickets.Count(x => x.Amenities.Service == c.HeaderText);
                               
                            }
                            r.Cells[c.Index].Value = cantidad;
                        }

                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
