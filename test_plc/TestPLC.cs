﻿using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace test_plc
{
	public partial class TestPLC : Form
	{
		// plc object
		//
		mcOMRON.OmronPLC plc;


		/// <summary>
		/// constructor
		/// </summary>
		public TestPLC()
		{
			InitializeComponent();

			// initielize a new plc object with tcp transport layer
			//
			this.plc = new mcOMRON.OmronPLC(mcOMRON.TransportType.Tcp);
		}
		

		/// <summary>
		/// try to connect to the plc
		/// </summary>
		private void Connect()
		{
			if (ip.Text == "") return;
			if (port.Text == "") return;

			try
			{
				// set ip:port for command layer, may cast to tcpFINSCommand to set ip and port
				//
				mcOMRON.tcpFINSCommand tcpCommand = ((mcOMRON.tcpFINSCommand)plc.FinsCommand);
				tcpCommand.SetTCPParams(IPAddress.Parse(ip.Text), Convert.ToInt32(port.Text));
				
				// connection
				//
				if (! plc.Connect())
				{
					throw new Exception(plc.LastError);
				}

				// set UI
				//
				bt_connect.Enabled = false;
				ip.Enabled = false;
				port.Enabled = false;
				bt_close.Enabled = true;
				bt_connection_data_read.Enabled = true;
				groupDM.Enabled = true;
				groupDMs.Enabled = true;
				groupDialogText.Enabled = true;
				groupCIO.Enabled = true;
				dialog.Clear();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Connect() error: " + ex.Message);
			}
		}
		

		/// <summary>
		/// shutdown the connection
		/// </summary>
		private void Shutdown()
		{
			plc.Close();

			bt_connect.Enabled = true;
			ip.Enabled = true;
			port.Enabled = true;
			bt_close.Enabled = false;
			bt_connection_data_read.Enabled = false;
			groupDM.Enabled = false;
			groupDMs.Enabled = false;
			groupDialogText.Enabled = false;
			groupCIO.Enabled = false;
		}
		

		/// <summary>
		/// exits this application
		/// </summary>
		private void Exit()
		{
			plc.Close();
			this.Close();
		}


		/// <summary>
		/// read a single DM
		/// </summary>
		private void ReadDM()
		{
			if (dm_position.Text == "") return;
			UInt16 dmval=0;

			try
			{
				if (! plc.ReadDM(Convert.ToUInt16(dm_position.Text), ref dmval))
				{
					throw new Exception(plc.LastError);
				}

				dm_value.Text = dmval.ToString();

				dialog.Text = plc.LastDialog("READ DM");
				dialog.AppendText("DM VALUE: " + dmval.ToString());
			}
			catch (Exception ex)
			{
				MessageBox.Show("ReadDM() Error: " + ex.Message);
			}
		}


		/// <summary>
		/// read some continous DM
		/// </summary>
		private void ReadDMs()
		{
			if (dms_position.Text == "" || dms_count.Text == "") return;
			UInt16 dmscount = 0;
			UInt16 [] data;

			try
			{
				dmscount = Convert.ToUInt16(dms_count.Text);
				data = new UInt16[dmscount];

				if (!plc.ReadDMs(Convert.ToUInt16(dms_position.Text), ref data, dmscount))
				{
					throw new Exception(plc.LastError);
				}

				dialog.Text = plc.LastDialog("READ DM's");
			}
			catch (Exception ex)
			{
				MessageBox.Show("ReadDMs() error: " + ex.Message);
			}
		}


		/// <summary>
		/// Controller data read
		/// </summary>
		private void ControllerDataRead()
		{
			try
			{
				if (!plc.finsConnectionDataRead(0))
				{
					throw new Exception(plc.LastError);
				}

				dialog.Text = plc.LastDialog("CONTROLLER DATA READ");
				dialog.AppendText("CONTROLLER: " + Encoding.ASCII.GetString(plc.FinsCommand.Response, 0, 20));
				dialog.AppendText(Environment.NewLine);
				dialog.AppendText("VERSION: " + Encoding.ASCII.GetString(plc.FinsCommand.Response, 20, 20));
			}
			catch (Exception ex)
			{
				MessageBox.Show("ControllerDataRead() error: " + ex.Message);
			}
		}


		/// <summary>
		/// write a single DM
		/// </summary>
		private void WriteDM()
		{
			if (dm_position.Text == "") return;
			UInt16 dmval = 0;

			try
			{
				dmval = Convert.ToUInt16(dm_value.Text);

				if (MessageBox.Show("This action will write some memory area of your PLC.\n\nContinue anyway?"
								, "OMRON PLC text"
								, MessageBoxButtons.OKCancel
								, MessageBoxIcon.Question
								, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.OK)
				{
					return;
				}

				if (!plc.WriteDM(Convert.ToUInt16(dm_position.Text), dmval))
				{
					throw new Exception(plc.LastError);
				}

				dialog.Text = plc.LastDialog("WRITE DM");
			}
			catch (Exception ex)
			{
				MessageBox.Show("WriteDM() error: " + ex.Message);
			}
		}


		/// <summary>
		/// clear some continous DM
		/// </summary>
		private void ClearDMs()
		{
			if (dms_position.Text == "" || dms_count.Text == "") return;
			UInt16 dmscount = 0;

			try
			{
				dmscount = Convert.ToUInt16(dms_count.Text);

				if (MessageBox.Show("This action will write some memory area of your PLC.\n\nContinue anyway?"
								, "OMRON PLC text"
								, MessageBoxButtons.OKCancel
								, MessageBoxIcon.Question
								, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.OK)
				{
					return;
				}

				if (!plc.ClearDMs(Convert.ToUInt16(dms_position.Text), dmscount))
				{
					throw new Exception(plc.LastError);
				}

				dialog.Text = plc.LastDialog("CLEAR DM's");
			}
			catch (Exception ex)
			{
				MessageBox.Show("ClearDMs() error: " + ex.Message);
			}
		}



		/// <summary>
		/// read a CIO bit
		/// </summary>
		private void ReadCIOBit()
		{
			if (this.cio_position.Text == "") return;
			if (this.cio_bit.Text == "") return;
			Byte cioval = 0;

			try
			{
				if (!plc.ReadCIOBit(Convert.ToUInt16(cio_position.Text), Convert.ToByte(cio_bit.Text), ref cioval))
				{
					throw new Exception(plc.LastError);
				}

				cio_value.Text = cioval.ToString();

				dialog.Text = plc.LastDialog("READ CIO Bit");
				dialog.AppendText("CIO Bit VALUE: " + cioval.ToString());
			}
			catch (Exception ex)
			{
				MessageBox.Show("ReadCIOBit() Error: " + ex.Message);
			}
		}



		/// <summary>
		/// write a single CIO Bit
		/// </summary>
		private void WriteCIOBit()
		{
			if (this.cio_position.Text == "") return;
			if (this.cio_bit.Text == "") return;
			Byte cioval = 0;

			try
			{
				cioval = Convert.ToByte(cio_value.Text);

				if (cioval != 0 && cioval != 1)
					throw new Exception("WriteCIOBit requires a range values between 0-1");

				if (MessageBox.Show("This action will write some memory area of your PLC.\n\nContinue anyway?"
								, "OMRON PLC text"
								, MessageBoxButtons.OKCancel
								, MessageBoxIcon.Question
								, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.OK)
				{
					return;
				}
				if (!plc.WriteCIOBit(Convert.ToUInt16(cio_position.Text), Convert.ToByte(cio_bit.Text), cioval))
				{
					throw new Exception(plc.LastError);
				}
				dialog.Text = plc.LastDialog("WRITE CIO Bit");
			}
			catch (Exception ex)
			{
				MessageBox.Show("WriteCIOBit() error: " + ex.Message);
			}
		}

        // Chuyển đổi một giá trị float thành 2 thanh ghi UInt16
        private UInt16[] FloatToUInt16(float value)
        {
            byte[] byteArray = BitConverter.GetBytes(value); // Chuyển float thành mảng byte
            UInt16[] result = new UInt16[2];

            // Mỗi thanh ghi 16-bit sẽ chứa 2 byte
            result[0] = BitConverter.ToUInt16(byteArray, 0); // Lấy 2 byte đầu
            result[1] = BitConverter.ToUInt16(byteArray, 2); // Lấy 2 byte sau

            return result;
        }
        // Chuyển đổi 2 thanh ghi UInt16 thành một giá trị float
        private float UInt16ToFloat(UInt16[] values)
        {
            byte[] byteArray = new byte[4];
            // Chuyển đổi 2 giá trị UInt16 thành 4 byte
            BitConverter.GetBytes(values[0]).CopyTo(byteArray, 0); // 2 byte đầu
            BitConverter.GetBytes(values[1]).CopyTo(byteArray, 2); // 2 byte sau
            return BitConverter.ToSingle(byteArray, 0); // Chuyển mảng byte thành float
        }
        // Ghi một giá trị float vào PLC
        private void WriteFloatDM(string dm_position, float value)
        {
            if (dm_position == "") return;

            try
            {
                UInt16[] dmValues = FloatToUInt16(value); // Chuyển đổi float thành 2 thanh ghi

                if (MessageBox.Show("This action will write a float value to your PLC.\n\nContinue anyway?"
                                    , "OMRON PLC float write"
                                    , MessageBoxButtons.OKCancel
                                    , MessageBoxIcon.Question
                                    , MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                // Ghi 2 thanh ghi vào PLC
                if (!plc.WriteDM(Convert.ToUInt16(dm_position), dmValues[0]) || !plc.WriteDM((UInt16)(Convert.ToUInt16(dm_position) + 1), dmValues[1]))
                {
                    throw new Exception(plc.LastError);
                }
                dialog.Text = plc.LastDialog("WRITE DM FLOAT");
            }
            catch (Exception ex)
            {
                MessageBox.Show("WriteFloatDM() error: " + ex.Message);
            }
        }
        // Đọc một giá trị float từ PLC
        private void ReadFloatDM(string dm_position)
        {
            if (dm_position == "") return;

            try
            {
                UInt16 dmVal1 = 0;
                UInt16 dmVal2 = 0;

                // Đọc 2 thanh ghi liên tiếp
                if (!plc.ReadDM(Convert.ToUInt16(dm_position), ref dmVal1) || !plc.ReadDM((UInt16)(Convert.ToUInt16(dm_position) + 1), ref dmVal2))
                {
                    throw new Exception(plc.LastError);
                }

                // Kết hợp 2 thanh ghi thành một giá trị float
                float value = UInt16ToFloat(new UInt16[] { dmVal1, dmVal2 });

                dm_value.Text = value.ToString(); // Hiển thị giá trị float
                dialog.Text = plc.LastDialog("READ DM FLOAT");
                dialog.AppendText("DM VALUE (FLOAT): " + value.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("ReadFloatDM() error: " + ex.Message);
            }
        }


        #region **** controls events

        private void bt_connect_Click(object sender, EventArgs e)
		{
			Connect();
		}

		private void bt_close_Click(object sender, EventArgs e)
		{
			Shutdown();
		}

		private void bt_exit_Click(object sender, EventArgs e)
		{
			Exit();
		}

		private void bt_read_dm_Click(object sender, EventArgs e)
		{
			ReadDM();
		}

		private void bt_write_dm_Click(object sender, EventArgs e)
		{
			WriteDM();
		}

		private void bt_read_dms_Click(object sender, EventArgs e)
		{
			ReadDMs();
		}

		private void bt_clear_dms_Click(object sender, EventArgs e)
		{
			ClearDMs();
		}

		private void bt_connection_data_read_Click(object sender, EventArgs e)
		{
			ControllerDataRead();
		}

		private void bt_read_cio_Click(object sender, EventArgs e)
		{
			ReadCIOBit();
		}

		private void bt_write_cio_Click(object sender, EventArgs e)
		{
			WriteCIOBit();
		}

        #endregion

        private void btnReadFloat_Click(object sender, EventArgs e)
        {
            ReadFloatDM(dm_position.Text);
        }

        private void btnWriteFloat_Click(object sender, EventArgs e)
        {
            // Chuyển giá trị float từ textbox (hoặc bất kỳ input nào) thành float để ghi vào PLC
            float valueToWrite;
            if (float.TryParse(dm_value.Text, out valueToWrite)) // dm_value là ô textbox bạn nhập giá trị float
            {
                // Gọi hàm ghi float vào PLC
                WriteFloatDM(dm_position.Text, valueToWrite);
            }
            else
            {
                MessageBox.Show("Invalid float value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
