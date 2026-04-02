using AnyTeller.Models;
using AnyTeller.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AnyTeller.Forms
{
    public partial class TransactionForm : Form
    {
        private SessionManager _sessionManager;
        private ChaosControlFactory _chaosFactory;
        
        // UI Controls
        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnLookup;
        private GroupBox grpTransaction; // The container for dynamic fields
        private Button btnSubmit;
        private Label lblStats;
        private System.Windows.Forms.Timer tmrSession;
        
        // State
        private UserRecord _currentRecord;
        private Dictionary<string, Control> _fieldControls = new Dictionary<string, Control>();
        private DateTime _startTime;

        public TransactionForm(List<UserRecord> records)
        {
            InitializeComponent();
            _sessionManager = new SessionManager(records);
            _chaosFactory = new ChaosControlFactory();
            
            // Start Timer
            _startTime = DateTime.Now;
            tmrSession = new System.Windows.Forms.Timer();
            tmrSession.Interval = 1000;
            tmrSession.Tick += TmrSession_Tick;
            tmrSession.Start();
            
            StartRound();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 650);
            this.Text = "AnyTeller - Transaction Window";
            this.StartPosition = FormStartPosition.CenterParent;

            // Search Area
            lblSearch = new Label { Text = "Username Lookup:", Location = new Point(20, 20), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(160, 18), Width = 150 };
            btnLookup = new Button { Text = "Lookup", Location = new Point(330, 17), Width = 100, Height = 30 };
            btnLookup.Click += BtnLookup_Click;

            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnLookup);

            // Container for Chaos
            grpTransaction = new GroupBox
            {
                Location = new Point(20, 80), // Moved down to avoid overlap with search bar
                Size = new Size(740, 450),
                Text = "Session Details",
                Visible = false 
            };
            this.Controls.Add(grpTransaction);

            // Submit Button
            btnSubmit = new Button
            {
                Text = "Submit Transaction",
                Location = new Point(630, 550), // Positioned clearly below the groupbox
                Size = new Size(130, 35), // Slightly larger
                Visible = false
            };
            btnSubmit.Click += BtnSubmit_Click;
            this.Controls.Add(btnSubmit);

            // Stats Label
            lblStats = new Label 
            { 
                Text = "Time: 0s | Score: 0/0", 
                Location = new Point(500, 20), 
                AutoSize = true, 
                Font = new Font(this.Font, FontStyle.Bold) 
            };
            this.Controls.Add(lblStats);
        }

        private void TmrSession_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _startTime;
            lblStats.Text = $"Time: {elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2} | Score: {_sessionManager.Score}/{_sessionManager.TotalProcessed}";
        }

        private void StartRound()
        {
            if (_sessionManager.IsSessionComplete())
            {
                MessageBox.Show($"Session Complete!\nRecords Processed: {_sessionManager.TotalProcessed}\nFields Accurately Matched: {_sessionManager.Score}", "Result");
                this.Close();
                return;
            }

            // Still default to next in queue if they just start the round, 
            // but they can override it by looking up another user.
            _currentRecord = _sessionManager.GetCurrentRecord();
            
            // Reset UI
            txtSearch.Text = "";
            grpTransaction.Controls.Clear();
            grpTransaction.Visible = false;
            btnSubmit.Visible = false;
            _fieldControls.Clear();
            
            // Enable search
            txtSearch.Enabled = true;
            btnLookup.Enabled = true;
        }

        private void BtnLookup_Click(object sender, EventArgs e)
        {
            var targetRecord = _sessionManager.Records.FirstOrDefault(r => string.Equals(r.Username, txtSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (targetRecord != null)
            {
                _currentRecord = targetRecord;
                SetupChaosForm(true);
            }
            else
            {
                MessageBox.Show("User not found in the records.", "Lookup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupChaosForm(bool userExists)
        {
            txtSearch.Enabled = false;
            btnLookup.Enabled = false;

            grpTransaction.Visible = true;
            btnSubmit.Visible = true;

            Random rnd = new Random();
            string[] addTitles = { "Update User Profile", "Modify Member", "Edit Entry", "Adjust Record" };
            grpTransaction.Text = addTitles[rnd.Next(addTitles.Length)];

            // Create TableLayoutPanel
            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.Dock = DockStyle.Fill;
            tlp.RowCount = 7; 
            tlp.ColumnCount = 2;
            
            // Col 1: Labels
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            // Col 2: Inputs
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            tlp.Padding = new Padding(10);
            
            var fields = new Dictionary<string, string>
            {
                { "First Name", _currentRecord.FirstName },
                { "Contact Number", _currentRecord.ContactNumber },
                { "Manager", _currentRecord.Manager },
                { "Email", _currentRecord.Email },
                { "Job Description", _currentRecord.JobDescription },
                { "Level", _currentRecord.Level }
            };

            // List of (Label, Input, IsPlaceholder)
            List<(Control lbl, Control input, bool isPlaceholder)> generatedRows = new List<(Control, Control, bool)>();

            foreach (var kvp in fields)
            {
                // 1. Get Random Synonym
                string synonym = GetRandomSynonym(kvp.Key, rnd);

                // 2. Randomly decide: Label vs Placeholder (50/50)
                bool usePlaceholder = rnd.Next(2) == 0;
                
                Label lbl;
                // Pass the SYNONYM to the factory so it sets the text/placeholder correctly
                Control input = _chaosFactory.CreateInputControl(synonym, "", usePlaceholder, out lbl);
                
                // Style the label if it exists
                if (lbl != null)
                {
                    lbl.TextAlign = ContentAlignment.MiddleLeft;
                    lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    lbl.AutoSize = true;
                    lbl.Margin = new Padding(0, 5, 10, 5);
                }

                input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                input.Width = 300;

                _fieldControls[kvp.Key] = input;
                generatedRows.Add((lbl, input, usePlaceholder));
            }

            // Generate "Reason" Widget (Special Case)
            var reasonWidget = CreateReasonWidgetControls(rnd);
            generatedRows.Add(reasonWidget);

            // Shuffle rows
            generatedRows = generatedRows.OrderBy(x => rnd.Next()).ToList();

            grpTransaction.Controls.Clear();
            grpTransaction.Controls.Add(tlp);
            
            int row = 0;
            foreach (var item in generatedRows)
            {
                tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                
                if (item.isPlaceholder)
                {
                    // If placeholder, span the input across both columns
                    // item.lbl is likely null or ignored
                    tlp.Controls.Add(item.input, 0, row);
                    tlp.SetColumnSpan(item.input, 2);
                }
                else
                {
                    // Standard Label + Input
                    tlp.Controls.Add(item.lbl, 0, row);
                    tlp.Controls.Add(item.input, 1, row);
                }
                row++;
            }
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        }

        private string GetRandomSynonym(string fieldName, Random rnd)
        {
            // Dictionary of synonyms
            var synonyms = new Dictionary<string, string[]>
            {
                { "First Name", new[] { "First Name", "Given Name", "Forename", "Name", "Member Name" } },
                { "Contact Number", new[] { "Contact Number", "Phone", "Mobile No", "Cell", "Telephone", "Contact #" } },
                { "Manager", new[] { "Manager", "Supervisor", "Team Lead", "Boss", "Reports To", "Direct Report" } },
                { "Email", new[] { "Email", "E-mail Address", "Mail", "Contact Email", "Electronic Mail" } },
                { "Job Description", new[] { "Job Description", "Role", "Position", "Title", "Designation", "Job Title" } },
                { "Level", new[] { "Level", "Grade", "Rank", "Seniority", "Tier", "Band" } },
                { "Leave Reason", new[] { "Leave Reason", "Reason", "Justification", "Cause", "Explanation", "Why?" } }
            };

            if (synonyms.ContainsKey(fieldName))
            {
                var options = synonyms[fieldName];
                return options[rnd.Next(options.Length)];
            }
            return fieldName;
        }

        private (Control lbl, Control input, bool isPlaceholder) CreateReasonWidgetControls(Random rnd)
        {
            int round = _sessionManager.TotalProcessed + 1; 
            int variant = (round - 1) % 3; 

            string originalField = "Leave Reason";
            string synonym = GetRandomSynonym(originalField, rnd);
            string value = _currentRecord.LeaveReason;

             // Reason widget usually doesn't support "Placeholder" naturally for ComboBox/Radio
             // So we might enforce Label for Cbox/Radio, but allow Placeholder for TextBox?
             // Variant 0 is TextBox.
            
            bool usePlaceholder = false; 
            if (variant == 0) usePlaceholder = rnd.Next(2) == 0;

            Label lbl = null;
            if (!usePlaceholder)
            {
                lbl = new Label { Text = synonym, AutoSize = true, Name = "lbl_" + ChaosControlFactory.GetRandomString(8) };
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                lbl.Margin = new Padding(0, 5, 10, 5);
            }

            Control inputControl = null;

            if (variant == 0) // TextBox
            {
                string randomId = ChaosControlFactory.GetRandomString(8);
                var txt = new TextBox { Text = "", Name = "txt_" + randomId };
                if (usePlaceholder)
                {
                    txt.PlaceholderText = synonym;
                }
                inputControl = txt;
            }
            else if (variant == 1) // ComboBox
            {
                var cb = new ComboBox { Name = "cb_" + ChaosControlFactory.GetRandomString(8), DropDownStyle = ComboBoxStyle.DropDownList };
                cb.SelectedIndex = -1;
                cb.Items.AddRange(new object[] { "Sick Leave", "Vacation", "Personal", "Maternity", "Sabbatical", "Beet Farming", "Prank War", "Art School", "Business School", "Retirement" });
                
                // ComboBox doesn't really have a placeholder text property that persists after selection?
                // We'll stick to Label for ComboBox (usePlaceholder = false forced above implicitly? No only check variant 0).
                // Let's enforce Label for non-TextBox for simplicity/usability.
                usePlaceholder = false;
                if (lbl == null) // Re-create label if we switched back
                {
                    lbl = new Label { Text = synonym, AutoSize = true, Name = "lbl_" + ChaosControlFactory.GetRandomString(8) };
                    lbl.TextAlign = ContentAlignment.MiddleLeft;
                    lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    lbl.Margin = new Padding(0, 5, 10, 5);
                }
                
                inputControl = cb;
            }
            else // FlowLayoutPanel with RadioButtons
            {
                var flp = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, Name = "pnl_" + ChaosControlFactory.GetRandomString(8) };
                string[] options = { "Sick Leave", "Vacation", "Personal", "Other" };
                
                var allOptions = options.ToList();
                if (!allOptions.Contains(value)) allOptions.Add(value);

                foreach (var opt in allOptions)
                {
                    var rb = new RadioButton { Text = opt, AutoSize = true, Checked = false, Name = "rb_" + ChaosControlFactory.GetRandomString(6) };
                    flp.Controls.Add(rb);
                }
                
                usePlaceholder = false;
                if (lbl == null)
                {
                    lbl = new Label { Text = synonym, AutoSize = true, Name = "lbl_" + ChaosControlFactory.GetRandomString(8) };
                    lbl.TextAlign = ContentAlignment.MiddleLeft;
                    lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    lbl.Margin = new Padding(0, 5, 10, 5);
                }
                
                inputControl = flp;
            }

            inputControl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _fieldControls[originalField] = inputControl; // Track using original key
            
            return (lbl, inputControl, usePlaceholder);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Validate
            bool accurate = true;
            foreach (var kvp in _fieldControls)
            {
                string expected = GetExpectedValue(kvp.Key);
                string actual = GetValueFromControl(kvp.Value);
                
                // Simple trimmed comparison
                if (!string.Equals(actual?.Trim(), expected?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    accurate = false;
                    break;
                }
            }

            // Generate Random Pop Up Message
            Random rnd = new Random();
            string title;
            string body;

            if (accurate)
            {
                string[] successTitles = { "Success", "Verified", "Entry Accepted", "Done", "Correct", "Validation Passed" };
                string[] successBodies = { "The data is correct.", "Great job, entry matches.", "Verification successful.", "Row accepted.", "Perfect match." };
                
                title = successTitles[rnd.Next(successTitles.Length)];
                body = successBodies[rnd.Next(successBodies.Length)];
                
                MessageBox.Show(body, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Only advance if accurate
                _sessionManager.SubmitRound(true);
                StartRound();
            }
            else
            {
                string[] failTitles = { "Error", "Mismatch", "Validation Failed", "Incorrect Data", "Rejected", "Alert" };
                string[] failBodies = { "The input data does not match the record.", "Validation error: Check your entries.", "Incorrect values detected.", "Data mismatch found.", "Please review the form data." };
                
                title = failTitles[rnd.Next(failTitles.Length)];
                body = failBodies[rnd.Next(failBodies.Length)];
                
                MessageBox.Show(body, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Do NOT advance - User must retry
            }
        }

        private string GetExpectedValue(string field)
        {
            switch (field)
            {
                case "First Name": return _currentRecord.FirstName;
                case "Contact Number": return _currentRecord.ContactNumber;
                case "Manager": return _currentRecord.Manager;
                case "Email": return _currentRecord.Email;
                case "Job Description": return _currentRecord.JobDescription;
                case "Level": return _currentRecord.Level;
                case "Leave Reason": return _currentRecord.LeaveReason;
                default: return "";
            }
        }

        private string GetValueFromControl(Control ctl)
        {
            if (ctl is TextBox txt) return txt.Text;
            if (ctl is ComboBox cb) return cb.Text;
            if (ctl is FlowLayoutPanel flp)
            {
                foreach (Control c in flp.Controls)
                {
                    if (c is RadioButton rb && rb.Checked) return rb.Text;
                }
            }
            return "";
        }
    }
}
