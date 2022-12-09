namespace GithubActors
{
  partial class GithubAuth
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.label1 = new System.Windows.Forms.Label();
            this.tbOAuth = new System.Windows.Forms.TextBox();
            this.lblAuthStatus = new System.Windows.Forms.Label();
            this.linkGhLabel = new System.Windows.Forms.LinkLabel();
            this.btnAuthenticate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "GitHub Access Token";
            // 
            // tbOAuth
            // 
            this.tbOAuth.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tbOAuth.Location = new System.Drawing.Point(222, 7);
            this.tbOAuth.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbOAuth.Name = "tbOAuth";
            this.tbOAuth.Size = new System.Drawing.Size(442, 24);
            this.tbOAuth.TabIndex = 1;
            // 
            // lblAuthStatus
            // 
            this.lblAuthStatus.AutoSize = true;
            this.lblAuthStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAuthStatus.Location = new System.Drawing.Point(218, 38);
            this.lblAuthStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAuthStatus.Name = "lblAuthStatus";
            this.lblAuthStatus.Size = new System.Drawing.Size(87, 18);
            this.lblAuthStatus.TabIndex = 2;
            this.lblAuthStatus.Text = "lblGHStatus";
            this.lblAuthStatus.Visible = false;
            // 
            // linkGhLabel
            // 
            this.linkGhLabel.AutoSize = true;
            this.linkGhLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.linkGhLabel.Location = new System.Drawing.Point(173, 148);
            this.linkGhLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkGhLabel.Name = "linkGhLabel";
            this.linkGhLabel.Size = new System.Drawing.Size(273, 18);
            this.linkGhLabel.TabIndex = 3;
            this.linkGhLabel.TabStop = true;
            this.linkGhLabel.Text = "How to get a GitHub Access Token";
            this.linkGhLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGhLabel_LinkClicked);
            // 
            // btnAuthenticate
            // 
            this.btnAuthenticate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAuthenticate.Location = new System.Drawing.Point(250, 93);
            this.btnAuthenticate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAuthenticate.Name = "btnAuthenticate";
            this.btnAuthenticate.Size = new System.Drawing.Size(159, 37);
            this.btnAuthenticate.TabIndex = 4;
            this.btnAuthenticate.Text = "Authenticate";
            this.btnAuthenticate.UseVisualStyleBackColor = true;
            this.btnAuthenticate.Click += new System.EventHandler(this.btnAuthenticate_Click);
            // 
            // GithubAuth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 179);
            this.Controls.Add(this.btnAuthenticate);
            this.Controls.Add(this.linkGhLabel);
            this.Controls.Add(this.lblAuthStatus);
            this.Controls.Add(this.tbOAuth);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "GithubAuth";
            this.Text = "Sign in to GitHub";
            this.Load += new System.EventHandler(this.GithubAuth_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox tbOAuth;
    private System.Windows.Forms.Label lblAuthStatus;
    private System.Windows.Forms.LinkLabel linkGhLabel;
    private System.Windows.Forms.Button btnAuthenticate;
  }
}