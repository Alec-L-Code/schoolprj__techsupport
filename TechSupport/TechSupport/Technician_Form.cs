using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechSupport
{
    public partial class Technician_Form : Form
    {
        public Technician_Form()
        {
            InitializeComponent();
        }

        private void techniciansBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            LeaveAddNewItemButton_Click(sender, e);
            this.Validate();
            try
            {
                this.techniciansBindingSource.EndEdit();
                this.tableAdapterManager.UpdateAll(this.techSupportDataSet);
            }
            catch (DBConcurrencyException)
            {
                MessageBox.Show("A concurrency error occurred." +
                    "Some rows were not updated", "Concurrency Exception");
                this.techniciansTableAdapter.Fill(this.techSupportDataSet.Technicians);
            }
            catch (DataException ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
                techniciansBindingSource.CancelEdit();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error # " + ex.Number +
                    ": " + ex.Message, ex.GetType().ToString());
            }
        }

        private void Technician_Form_Load(object sender, EventArgs e)
        {
            this.Validate();
            try
            {
                this.techniciansTableAdapter.Fill(this.techSupportDataSet.Technicians);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error # " + ex.Number +
                    ": " + ex.Message, ex.GetType().ToString());
            }
        }
        // ALL INPUTS ARE RESTRICTED TO SIZE LIMITS RELATIVE TO WHAT THEY ARE. EMAIL: 250 CHARS, PNUMBER: 12 CHARS, NAME 120 CHARS.
        // Handles the Addition of a new row to the table in memory.
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            techIDTextBox.ReadOnly = false; // Allows the ID to be manual inputted (Not that it matters at all. Its auto gen'd)
            // Disabling all input except for the delete and save button. Automagically reenables upon successful save or delete.
            bindingNavigatorMoveFirstItem.Enabled = false;
            bindingNavigatorMovePreviousItem.Enabled = false;
            bindingNavigatorMoveLastItem.Enabled = false;
            bindingNavigatorMoveNextItem.Enabled = false;
            bindingNavigatorPositionItem.Enabled = false;
            bindingNavigatorAddNewItem.Enabled = false;
            techniciansBindingNavigatorSaveItem.Enabled = false;
        }
        // Called whenever any other button is pressed to keep the TechID readonly when not adding a new row.
        private void LeaveAddNewItemButton_Click(object sender, EventArgs e)
        {
            this.Validate();
            techIDTextBox.ReadOnly = true; // Disables the TechID Textbox.
            bindingNavigatorAddNewItem.Enabled = true; // Reenables the AddNewItem Navigator Icon.
            techniciansBindingNavigatorSaveItem.Enabled = true; // Reenables the Save button.
        }
        // Handles key presses for the technician name textbox. Insures only letters and white space is being entered
        // All of the key release and keydowns handle some sort of functionality associated with saving the new item into the table
        // Phone numbers must follow the format: 123-123-1234, names must not contain anything but letters and whitespace, and all textboxes must have at least one character
        private void TechnicianName_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && !char.IsControl(e.KeyChar)) { e.Handled = true; }
        }
        private void TechnicianName_TextBox_KeyRelease(object sender, KeyEventArgs e)
        {
            if (nameTextBox.TextLength == 0 || emailTextBox.TextLength == 0 || phoneTextBox.TextLength != 12)
            {
                techniciansBindingNavigatorSaveItem.Enabled = false;
            }
            else
            {
                techniciansBindingNavigatorSaveItem.Enabled = true;
            }
        }
        private void TechnicianEmail_TextBox_KeyRelease(object sender, KeyEventArgs e)
        {
            if (nameTextBox.TextLength == 0 || emailTextBox.TextLength == 0 || phoneTextBox.TextLength != 12)
            {
                techniciansBindingNavigatorSaveItem.Enabled = false;
            }
            else
            {
                techniciansBindingNavigatorSaveItem.Enabled = true;
            }
        }
        private void TechnicianPhoneNumber_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsWhiteSpace(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void TechnicianPhoneNumber_TextBox_KeyRelease(object sender, KeyEventArgs e)
        {
            if (phoneTextBox.TextLength != 12)
            {
                techniciansBindingNavigatorSaveItem.Enabled = false;
            }
            else
            {
                Match match = Regex.Match(phoneTextBox.Text, @"^\d{3}-\d{3}-\d{4}");
                if (match.Success)
                {
                    techniciansBindingNavigatorSaveItem.Enabled = true;
                }
            }
        }
    }
}
