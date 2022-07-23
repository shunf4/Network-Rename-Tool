using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using static Vanara.PInvoke.IpHlpApi;

namespace NetworkRenameTool {

    /// <summary>
    /// The main program window.
    /// </summary>
    public partial class MainForm : Form {

        /// <summary>
        /// Creates main window.
        /// </summary>
        public MainForm() {
            InitializeComponent();
            FeedProfiles();
            FeedNetworkAdapters();
        }

        /// <summary>
        /// Gets Windows network profiles from the Registry and feeds them into the grid.
        /// </summary>
        private void FeedProfiles() {
            NetworksGrid.Rows.Clear();
            using (var profilesKey = Registry.LocalMachine.OpenSubKey(ProfilesPath)) {
                foreach (var subkeyName in profilesKey.GetSubKeyNames()) {
                    using (var subkey = profilesKey.OpenSubKey(subkeyName)) {
                        NetworksGrid.Rows.Add(new[] { subkeyName, subkey.GetValue(ProfileNameKey), subkey.GetValue(DescriptionKey), System.Convert.ToInt32((subkey.GetValue(CategoryKey))) });
                    }
                }
            }
        }

        private void FeedNetworkAdapters()
        {
            IEnumerable<IP_ADAPTER_ADDRESSES> adapterAddresses_es = GetAdaptersAddresses();
            cmbNetworkAdapters.DataSource = Enumerable.ToList(adapterAddresses_es.Select(a => new KeyValuePair<string, IP_ADAPTER_ADDRESSES>(a.FriendlyName + " " + a.AdapterName + " " + $"0x{a.Luid.Value:X}", a)).ToList());
            cmbNetworkAdapters.ValueMember = "Value";
            cmbNetworkAdapters.DisplayMember = "Key";
            if (cmbNetworkAdapters.Items.Count > 0)
            {
                cmbNetworkAdapters.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles grid cell modification event.
        /// </summary>
        /// <param name="sender"><see cref="DataGridView"/>.</param>
        /// <param name="e">Event data.</param>
        private void NetworksGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0) {
                var grid = sender as DataGridView;
                var row = grid.Rows[e.RowIndex];
                var subkeyName = (string)row.Cells[0].Value;
                var targetKeyName = $"{ProfilesPath}\\{subkeyName}";
                string value;
                int valueInt32;
                bool int32ParseSuccess;
                switch (e.ColumnIndex) {
                    case 1:
                        value = (string)row.Cells[1].Value;
                        using (var subkey = Registry.LocalMachine.OpenSubKey(targetKeyName, true)) subkey.SetValue(ProfileNameKey, value);
                        break;
                    case 2:
                        value = (string)row.Cells[2].Value;
                        using (var subkey = Registry.LocalMachine.OpenSubKey(targetKeyName, true)) subkey.SetValue(DescriptionKey, value);
                        break;
                    case 3:
                        value = (string)row.Cells[3].Value;
                        int32ParseSuccess = System.Int32.TryParse(value, out valueInt32);
                        if (!int32ParseSuccess)
                        {
                            valueInt32 = 0;
                        }
                        using (var subkey = Registry.LocalMachine.OpenSubKey(targetKeyName, true)) subkey.SetValue(CategoryKey, valueInt32);
                        row.Cells[3].Value = valueInt32.ToString();
                        break;
                }
            }
        }

        /// <summary>
        /// Handles grid row delete event.
        /// </summary>
        /// <param name="sender"><see cref="DataGridView"/>.</param>
        /// <param name="e">Event data.</param>
        private void NetworksGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            var subkeyName = (string)e.Row.Cells[0].Value;
            using (var profilesKey = Registry.LocalMachine.OpenSubKey(ProfilesPath, true)) profilesKey.DeleteSubKey(subkeyName);
        }

        /// <summary>
        /// Handles grid key down event.
        /// </summary>
        /// <param name="sender"><see cref="DataGridView"/>.</param>
        /// <param name="e">Event data.</param>
        private void NetworksGrid_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == (Keys.Control | Keys.R)) FeedProfiles();
        }

        #region Constants

        private const string ProfilesPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkList\Profiles";
        private const string ProfileNameKey = "ProfileName";
        private const string DescriptionKey = "Description";
        private const string CategoryKey = "Category";

        #endregion

        private bool searchSetNetworkAdapterClassNdisDeviceType(string targetNetCfgInstanceId, uint val, out string restoringPowershellScript)
        {
            bool nwAdapterClassFound = false;
            restoringPowershellScript = "";
            using (var nwAdapterClasses = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}", true))
            {
                foreach (var classIndex in nwAdapterClasses.GetSubKeyNames())
                {
                    using (var classSubKey = nwAdapterClasses.OpenSubKey(classIndex, true))
                    {
                        var netCfgInstanceIdRaw = classSubKey.GetValue("NetCfgInstanceId");
                        if (netCfgInstanceIdRaw != null && netCfgInstanceIdRaw is string)
                        {
                            var netCfgInstanceId = (string)netCfgInstanceIdRaw;
                            if (netCfgInstanceId.ToLower().Equals(targetNetCfgInstanceId.ToLower()))
                            {
                                object oldValue = classSubKey.GetValue("*NdisDeviceType", 0);
                                restoringPowershellScript = $"New-ItemProperty -Path 'HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Class\\{{4d36e972-e325-11ce-bfc1-08002be10318}}\\{classIndex}' -Name '*NdisDeviceType' -Value {oldValue} -PropertyType DWORD -Force -Confirm:$False ;";
                                classSubKey.SetValue("*NdisDeviceType", val, RegistryValueKind.DWord);
                                nwAdapterClassFound = true;
                                break;
                            }
                        }
                    }
                }
            }
            return nwAdapterClassFound;
        }

        private void btnAddNetworkProfile_Click(object sender, System.EventArgs e)
        {
            var ifInfo = (IP_ADAPTER_ADDRESSES)cmbNetworkAdapters.SelectedValue;
            var fakeIp = System.Net.IPAddress.Parse(txbFakeIp.Text);
            var luidHash = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.BitConverter.GetBytes(ifInfo.Luid.Value)).Take(3).ToArray();
            var fakeMac = $"00-00-A3-{luidHash[0]:X2}-{luidHash[1]:X2}-{luidHash[2]:X2}";

            bool nwAdapterClassFound;
            string restoringNdisDeviceTypePwshScript;
            nwAdapterClassFound = searchSetNetworkAdapterClassNdisDeviceType(ifInfo.AdapterName, 0, out restoringNdisDeviceTypePwshScript);

            if (nwAdapterClassFound == false)
            {
                MessageBox.Show($@"NetCfgInstanceId={ifInfo.AdapterName} not found in HKLM\SYSTEM\CurrentControlSet\Control\Class");
                return;
            }

            var addingArpEntryPwshScript = $@"New-NetRoute -InterfaceIndex $(Get-NetAdapter | Where NetLuid -eq {ifInfo.Luid.Value}).InterfaceIndex -DestinationPrefix '0.0.0.0/0' -NextHop '{fakeIp}' -RouteMetric 9999 -Confirm:$False | Select ifIndex, RouteMetric, Store, DestinationPrefix, NextHop | Format-Table ;";
            var restoringArpEntryPwshScript = $@"Remove-NetRoute -InterfaceIndex $(Get-NetAdapter | Where NetLuid -eq {ifInfo.Luid.Value}).InterfaceIndex -DestinationPrefix '0.0.0.0/0' -NextHop '{fakeIp}' -RouteMetric 9999 -Confirm:$False ;";
            var addingGatewayRoutePwshScript = $@"New-NetNeighbor -InterfaceIndex $(Get-NetAdapter | Where NetLuid -eq {ifInfo.Luid.Value}).InterfaceIndex -IPAddress '{fakeIp}' -LinkLayerAddress '{fakeMac}' -State Permanent -Confirm:$False | Select ifIndex, Store, IPAddress, LinkLayerAddress | Format-Table ;";
            var restoringGatewayRoutePwshScript = $@"Remove-NetNeighbor -InterfaceIndex $(Get-NetAdapter | Where NetLuid -eq {ifInfo.Luid.Value}).InterfaceIndex -IPAddress '{fakeIp}' -LinkLayerAddress '{fakeMac}' -State Permanent -Confirm:$False ;";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"powershell.exe";
            startInfo.Arguments = $@"& {{ { addingArpEntryPwshScript } ; { addingGatewayRoutePwshScript } ; }}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();

            MessageBox.Show($"Powershell stdout\n=================\n" + output + "\n=================\n\nPowershell stderr\n=================\n" + errors + "\n=================");

            string restoringFilePath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + System.IO.Path.DirectorySeparatorChar +
                string.Join("_", ("Restore settings for " + ifInfo.FriendlyName + ".bat").Split(System.IO.Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

            System.IO.File.WriteAllLines(restoringFilePath,
                new string[]{
                    $@"powershell -Command Start-Process powershell -Verb runas -ArgumentList """"""{restoringGatewayRoutePwshScript.Replace("$", "`$")} {restoringArpEntryPwshScript.Replace("$", "`$")} {restoringNdisDeviceTypePwshScript.Replace("$", "`$")} pause ; """""""
                }
            );

            MessageBox.Show($"A script that can restore settings is generated at " + restoringFilePath + ".");

            MessageBox.Show($"You may need a reboot to make the changes to NdisDeviceType take effect.");
        }
    }

}