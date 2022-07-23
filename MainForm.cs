using System;
using System.Collections.Generic;
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
            cmbNetworkAdapters.DataSource = Enumerable.ToList(adapterAddresses_es.Select(a => new KeyValuePair<string, NET_LUID>(a.FriendlyName + " " + a.AdapterName + " " + $"0x{a.Luid.Value:X}", a.Luid)).ToList());
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

        private bool searchSetNetworkAdapterClassNdisDeviceType(string targetNetCfgInstanceId, uint val)
        {
            bool nwAdapterClassFound = false;
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
            var luid = (NET_LUID)cmbNetworkAdapters.SelectedValue;
            var ifInfo = GetAdaptersAddresses().Where(x => x.Luid.Equals(luid)).First();

            var unicastAddressList = ifInfo.UnicastAddresses.Where(ip => ip.Address.GetSOCKADDR().si_family == Vanara.PInvoke.Ws2_32.ADDRESS_FAMILY.AF_INET && !ip.Address.GetSOCKADDR().Ipv4.ToString().StartsWith("169.254")).Take(1).ToList();

            if (unicastAddressList.Count == 0)
            {
                MessageBox.Show("Seems no IPv4 address currently configured for this adapter. Please set one first!");
                return;
            }

            var ipv4Address = unicastAddressList[0].Address.GetSOCKADDR().Ipv4;

            var fakeIp = System.Net.IPAddress.Parse(txbFakeIp.Text);
            var ipBytes = fakeIp.GetAddressBytes();
            var fakeIpRaw = (uint)ipBytes[3] << 24;
            fakeIpRaw += (uint)ipBytes[2] << 16;
            fakeIpRaw += (uint)ipBytes[1] << 8;
            fakeIpRaw += (uint)ipBytes[0];

            Vanara.PInvoke.Win32Error result = Vanara.PInvoke.Win32Error.NO_ERROR;

            var luidHash = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.BitConverter.GetBytes(ifInfo.Luid.Value)).Take(3).ToArray();

            var ipnrPhysicalAddressStr = "";

            int i;
            for (i = 0; i < 8; ++i)
            {
                MIB_IPNET_ROW2 ipnr = default;
                ipnr.Address.si_family = Vanara.PInvoke.Ws2_32.ADDRESS_FAMILY.AF_INET;
                ipnr.Address.Ipv4.sin_addr.S_addr = fakeIpRaw;
                ipnr.InterfaceLuid.Value = ifInfo.Luid.Value;
                ipnr.PhysicalAddress = new byte[IF_MAX_PHYS_ADDRESS_LENGTH];
                ipnr.PhysicalAddress[0] = 0x00; // just make something up that's consistent and not part of this net
                ipnr.PhysicalAddress[1] = 0x00;
                ipnr.PhysicalAddress[2] = 0xA3;
                ipnr.PhysicalAddress[3] = luidHash[0];
                ipnr.PhysicalAddress[4] = luidHash[1];
                ipnr.PhysicalAddress[5] = luidHash[2];
                ipnrPhysicalAddressStr = System.BitConverter.ToString(ipnr.PhysicalAddress.Take(6).ToArray());
                ipnr.PhysicalAddressLength = 6;
                ipnr.State = NL_NEIGHBOR_STATE.NlnsPermanent;
                ipnr.Flags = MIB_IPNET_ROW2_FLAGS.IsRouther;
                ipnr.ReachabilityTime = 0; // LastReachable
                result = CreateIpNetEntry2(ref ipnr);
                if (result != Vanara.PInvoke.Win32Error.NO_ERROR)
                    Thread.Sleep(250);
                else break;
            }
            if (i == 8)
            {
                MessageBox.Show("CreateIpNetEntry2 Failure: " + result.ToString());
                return;
            }

            for (i = 0; i < 8; ++i)
            {
                MIB_IPFORWARD_ROW2 nr;
                InitializeIpForwardEntry(out nr);
                nr.InterfaceLuid.Value = ifInfo.Luid.Value;
                nr.DestinationPrefix.Prefix.si_family = Vanara.PInvoke.Ws2_32.ADDRESS_FAMILY.AF_INET; // rest is left as 0.0.0.0/0
                nr.NextHop.si_family = Vanara.PInvoke.Ws2_32.ADDRESS_FAMILY.AF_INET;
                nr.NextHop.Ipv4.sin_addr.S_addr = fakeIpRaw;
                nr.Metric = 9999; // do not use as real default route
                nr.Protocol = MIB_IPFORWARD_PROTO.MIB_IPPROTO_NETMGMT;

                result = CreateIpForwardEntry2(ref nr);
                if (result != Vanara.PInvoke.Win32Error.NO_ERROR)
                    Thread.Sleep(250);
                else break;
            }
            if (i == 8)
            {
                MessageBox.Show("CreateIpForwardEntry2 Failure: " + result.ToString());
                return;
            }

            bool nwAdapterClassFound;
            nwAdapterClassFound = searchSetNetworkAdapterClassNdisDeviceType(ifInfo.AdapterName, 0);

            if (nwAdapterClassFound == false)
            {
                MessageBox.Show($@"NetCfgInstanceId={ifInfo.AdapterName} not found in HKLM\SYSTEM\CurrentControlSet\Control\Class");
                return;
            }

            MessageBox.Show($"Successfully added ARP entry {fakeIp} ~ {ipnrPhysicalAddressStr} (On {ipv4Address}), and created default route 0.0.0.0/0 ~ {fakeIp}.\nYou may need to reboot your PC, set an IP address to this adapter and execute this operation again to assign a network profile.");
        }

        private void btnSetNdisDeviceType_Click(object sender, EventArgs e)
        {
            var luid = (NET_LUID)cmbNetworkAdapters.SelectedValue;
            var ifInfo = GetAdaptersAddresses().Where(x => x.Luid.Equals(luid)).First();

            bool nwAdapterClassFound;
            nwAdapterClassFound = searchSetNetworkAdapterClassNdisDeviceType(ifInfo.AdapterName, 1);

            if (nwAdapterClassFound == false)
            {
                MessageBox.Show($@"NetCfgInstanceId={ifInfo.AdapterName} not found in HKLM\SYSTEM\CurrentControlSet\Control\Class");
                return;
            }

            MessageBox.Show("Success. Please reboot your PC.");
        }
    }

}