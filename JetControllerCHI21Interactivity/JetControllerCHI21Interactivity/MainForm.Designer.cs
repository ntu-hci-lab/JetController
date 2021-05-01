namespace JetControllerCHI21Interactivity
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox_GameSelect = new System.Windows.Forms.GroupBox();
            this.button_StartDriving = new System.Windows.Forms.Button();
            this.button_StartHalfLife = new System.Windows.Forms.Button();
            this.groupBox_SelectHapticTechnique = new System.Windows.Forms.GroupBox();
            this.pictureBox_ShowHapticFigure = new System.Windows.Forms.PictureBox();
            this.radioButton_JetController = new System.Windows.Forms.RadioButton();
            this.radioButton_AdaptiveTrigger = new System.Windows.Forms.RadioButton();
            this.radioButton_Vibration = new System.Windows.Forms.RadioButton();
            this.radioButton_Disable = new System.Windows.Forms.RadioButton();
            this.groupBox_GameSelect.SuspendLayout();
            this.groupBox_SelectHapticTechnique.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ShowHapticFigure)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox_GameSelect
            // 
            this.groupBox_GameSelect.Controls.Add(this.button_StartDriving);
            this.groupBox_GameSelect.Controls.Add(this.button_StartHalfLife);
            this.groupBox_GameSelect.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox_GameSelect.Location = new System.Drawing.Point(13, 10);
            this.groupBox_GameSelect.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox_GameSelect.Name = "groupBox_GameSelect";
            this.groupBox_GameSelect.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox_GameSelect.Size = new System.Drawing.Size(650, 100);
            this.groupBox_GameSelect.TabIndex = 0;
            this.groupBox_GameSelect.TabStop = false;
            this.groupBox_GameSelect.Text = "Select Experience (Game)";
            // 
            // button_StartDriving
            // 
            this.button_StartDriving.Location = new System.Drawing.Point(346, 27);
            this.button_StartDriving.Name = "button_StartDriving";
            this.button_StartDriving.Size = new System.Drawing.Size(300, 60);
            this.button_StartDriving.TabIndex = 1;
            this.button_StartDriving.Text = "Driving with Tactile (Surface Texture) Feedback";
            this.button_StartDriving.UseVisualStyleBackColor = true;
            this.button_StartDriving.Click += new System.EventHandler(this.button_StartDriving_Click);
            // 
            // button_StartHalfLife
            // 
            this.button_StartHalfLife.Location = new System.Drawing.Point(10, 27);
            this.button_StartHalfLife.Name = "button_StartHalfLife";
            this.button_StartHalfLife.Size = new System.Drawing.Size(300, 60);
            this.button_StartHalfLife.TabIndex = 0;
            this.button_StartHalfLife.Text = "Shooting with Recoil Feedback";
            this.button_StartHalfLife.UseVisualStyleBackColor = true;
            this.button_StartHalfLife.Click += new System.EventHandler(this.button_StartHalfLife_Click);
            // 
            // groupBox_SelectHapticTechnique
            // 
            this.groupBox_SelectHapticTechnique.Controls.Add(this.pictureBox_ShowHapticFigure);
            this.groupBox_SelectHapticTechnique.Controls.Add(this.radioButton_JetController);
            this.groupBox_SelectHapticTechnique.Controls.Add(this.radioButton_AdaptiveTrigger);
            this.groupBox_SelectHapticTechnique.Controls.Add(this.radioButton_Vibration);
            this.groupBox_SelectHapticTechnique.Controls.Add(this.radioButton_Disable);
            this.groupBox_SelectHapticTechnique.Enabled = false;
            this.groupBox_SelectHapticTechnique.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox_SelectHapticTechnique.Location = new System.Drawing.Point(16, 118);
            this.groupBox_SelectHapticTechnique.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox_SelectHapticTechnique.Name = "groupBox_SelectHapticTechnique";
            this.groupBox_SelectHapticTechnique.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox_SelectHapticTechnique.Size = new System.Drawing.Size(650, 660);
            this.groupBox_SelectHapticTechnique.TabIndex = 2;
            this.groupBox_SelectHapticTechnique.TabStop = false;
            this.groupBox_SelectHapticTechnique.Text = "Select Haptic Feedback";
            // 
            // pictureBox_ShowHapticFigure
            // 
            this.pictureBox_ShowHapticFigure.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_ShowHapticFigure.Image")));
            this.pictureBox_ShowHapticFigure.Location = new System.Drawing.Point(27, 54);
            this.pictureBox_ShowHapticFigure.Name = "pictureBox_ShowHapticFigure";
            this.pictureBox_ShowHapticFigure.Size = new System.Drawing.Size(600, 600);
            this.pictureBox_ShowHapticFigure.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_ShowHapticFigure.TabIndex = 4;
            this.pictureBox_ShowHapticFigure.TabStop = false;
            // 
            // radioButton_JetController
            // 
            this.radioButton_JetController.AutoSize = true;
            this.radioButton_JetController.Location = new System.Drawing.Point(494, 22);
            this.radioButton_JetController.Name = "radioButton_JetController";
            this.radioButton_JetController.Size = new System.Drawing.Size(109, 23);
            this.radioButton_JetController.TabIndex = 3;
            this.radioButton_JetController.TabStop = true;
            this.radioButton_JetController.Text = "JetController";
            this.radioButton_JetController.UseVisualStyleBackColor = true;
            this.radioButton_JetController.CheckedChanged += new System.EventHandler(this.OnFeedbackApproachCheckedChanged);
            // 
            // radioButton_AdaptiveTrigger
            // 
            this.radioButton_AdaptiveTrigger.AutoSize = true;
            this.radioButton_AdaptiveTrigger.Location = new System.Drawing.Point(367, 12);
            this.radioButton_AdaptiveTrigger.Name = "radioButton_AdaptiveTrigger";
            this.radioButton_AdaptiveTrigger.Size = new System.Drawing.Size(84, 42);
            this.radioButton_AdaptiveTrigger.TabIndex = 2;
            this.radioButton_AdaptiveTrigger.TabStop = true;
            this.radioButton_AdaptiveTrigger.Text = "Adaptive\r\nTrigger";
            this.radioButton_AdaptiveTrigger.UseVisualStyleBackColor = true;
            this.radioButton_AdaptiveTrigger.CheckedChanged += new System.EventHandler(this.OnFeedbackApproachCheckedChanged);
            // 
            // radioButton_Vibration
            // 
            this.radioButton_Vibration.AutoSize = true;
            this.radioButton_Vibration.Location = new System.Drawing.Point(245, 22);
            this.radioButton_Vibration.Name = "radioButton_Vibration";
            this.radioButton_Vibration.Size = new System.Drawing.Size(86, 23);
            this.radioButton_Vibration.TabIndex = 1;
            this.radioButton_Vibration.TabStop = true;
            this.radioButton_Vibration.Text = "Vibration";
            this.radioButton_Vibration.UseVisualStyleBackColor = true;
            this.radioButton_Vibration.CheckedChanged += new System.EventHandler(this.OnFeedbackApproachCheckedChanged);
            // 
            // radioButton_Disable
            // 
            this.radioButton_Disable.AutoSize = true;
            this.radioButton_Disable.Checked = true;
            this.radioButton_Disable.Location = new System.Drawing.Point(75, 22);
            this.radioButton_Disable.Name = "radioButton_Disable";
            this.radioButton_Disable.Size = new System.Drawing.Size(143, 23);
            this.radioButton_Disable.TabIndex = 0;
            this.radioButton_Disable.TabStop = true;
            this.radioButton_Disable.Text = "Disable All Haptic";
            this.radioButton_Disable.UseVisualStyleBackColor = true;
            this.radioButton_Disable.CheckedChanged += new System.EventHandler(this.OnFeedbackApproachCheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 781);
            this.Controls.Add(this.groupBox_SelectHapticTechnique);
            this.Controls.Add(this.groupBox_GameSelect);
            this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JetController CHI\'21 Interactivity Demo";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox_GameSelect.ResumeLayout(false);
            this.groupBox_SelectHapticTechnique.ResumeLayout(false);
            this.groupBox_SelectHapticTechnique.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ShowHapticFigure)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_GameSelect;
        private System.Windows.Forms.Button button_StartDriving;
        private System.Windows.Forms.Button button_StartHalfLife;
        private System.Windows.Forms.GroupBox groupBox_SelectHapticTechnique;
        private System.Windows.Forms.RadioButton radioButton_JetController;
        private System.Windows.Forms.RadioButton radioButton_AdaptiveTrigger;
        private System.Windows.Forms.RadioButton radioButton_Vibration;
        private System.Windows.Forms.RadioButton radioButton_Disable;
        private System.Windows.Forms.PictureBox pictureBox_ShowHapticFigure;
    }
}

