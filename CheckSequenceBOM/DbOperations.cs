using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Npgsql;

namespace CheckSequenceBOM
{
    class DbOperations
    {
        private const String SERVER = "localhost";
        private const String PORT = "5432";
        private const String USER = "PRODUCTION";
        private const String PASSWORD = "eurobag";
        private const String DATABASE = "PROD_TRACING";
        private NpgsqlConnection Connection = null;
        public String STATUS = "n.a.";

        public class PartinBom
        {
            // to represent PN + its description in the list 
            public string PartNumber { get; set; }

            public string Name { get; set; }
        }


        private void DatabaseInit()
        {
            //create connection object
            Connection = new NpgsqlConnection(
                "Server=" + SERVER + ";" +
                "Port=" + PORT + ";" +
                "User Id=" + USER + ";" +
                "Password=" + PASSWORD + ";" +
                "Database=" + DATABASE + ";"
                );
        }
        private void showError(NpgsqlException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void openConnection()
        {
            this.DatabaseInit();

            try
            {
                Connection.Open();
            }
            catch (NpgsqlException ex)
            {
                showError(ex);
            }
        }
        public void closeConnection()
        {
            this.DatabaseInit();
            try
            {
                Connection.Close();

            }
            catch (NpgsqlException ex)
            {
                showError(ex);
            }
        }



        public string ConnectionState()
        {

            if (Connection.State == System.Data.ConnectionState.Open)
                return "OK";
            else
                return "NOK";
        }


        public string getATraQLineID()
        {
          

            NpgsqlCommand getLineIdQuery = new NpgsqlCommand("SELECT stocpf_line from t_stocpf limit 1", Connection);
            NpgsqlDataReader dataFromT_stocpf = getLineIdQuery.ExecuteReader();
            string ATraQLineId;
    
            if (dataFromT_stocpf.HasRows & dataFromT_stocpf.Read())
                ATraQLineId = dataFromT_stocpf[0].ToString();
            else
                ATraQLineId = "xxxx";

            dataFromT_stocpf.Close();
            return ATraQLineId;

        }


        public List<string> getSequenceFamilies()
        {
                   List<string> SequenceFamilies = new List<string>();

            NpgsqlCommand getFamQuery = new NpgsqlCommand("SELECT * from t_nomen where et_refakf_comp is null and nomen_fin_validite is null order by et_refakf_se desc", Connection);
            NpgsqlDataReader dataFromT_nomen = getFamQuery.ExecuteReader();

            //SequenceFamilies.Add("Select a SW familly");

            if (dataFromT_nomen.HasRows)
            {
                while (dataFromT_nomen.Read())
                {
                    SequenceFamilies.Add(dataFromT_nomen[2].ToString());
                }
                dataFromT_nomen.Close();
            }
            return SequenceFamilies;

        }



        public List<string> getSequencesToBuild()
        {
            List<string> SequencesToBuild = new List<string>();
            string querytext = "WITH T0 as (select distinct id_trace_order from t_trace_order) " +
                           "SELECT T0.id_trace_order,T1.trace_order_value, T2.trace_order_value, T3.trace_order_value " +
                           "from T0 " +
                           "join t_trace_order T1 on T0.id_trace_order = T1.id_trace_order and T1.et_order_carord = '15' " +
                           "join t_trace_order T2 on T1.id_trace_order = T2.id_trace_order and T2.et_order_carord = '18' " +
                           "join t_trace_order T3 on T1.id_trace_order = T3.id_trace_order and T3.et_order_carord = '11' " +
                            "order by T0.id_trace_order;";

            //string querytext = "with T0 as (select distinct id_trace_order from t_trace_order) " +
            //    "select T0.id_trace_order, T1.trace_order_value from t_trace_order T1 " +
            //    "join T0 on T0.id_trace_order = T1.id_trace_order";

            NpgsqlCommand getSequenceQuery = new NpgsqlCommand(querytext, Connection);
            NpgsqlDataReader dataFromT_trace_order = getSequenceQuery.ExecuteReader();

            //SequencesToBuild.Add("Select a sequence");

            if (dataFromT_trace_order.HasRows)
            {
                while (dataFromT_trace_order.Read())
                {
                    SequencesToBuild.Add(dataFromT_trace_order[0].ToString()  + "; " + dataFromT_trace_order[1].ToString() + "; " + dataFromT_trace_order[2].ToString() + "; " + dataFromT_trace_order[3].ToString());
                }
                dataFromT_trace_order.Close();
            }
            return SequencesToBuild;
                       
         }

        public List<string> getSequnecesRacks()
        {
            List<string> SequnecesRacks = new List<string>();
            NpgsqlCommand getData = new NpgsqlCommand("select distinct trace_order_value from t_trace_order where et_order_carord = '11' order by trace_order_value asc", Connection);
            NpgsqlDataReader dataFromTable = getData.ExecuteReader();

            if (dataFromTable.HasRows)
            {
                while (dataFromTable.Read())
                {
                    SequnecesRacks.Add(dataFromTable[0].ToString());
                }
                dataFromTable.Close();
            }
            return SequnecesRacks;
        }

        public List<PartinBom> getBillOfMaterial(string FamName)
        {
            List<PartinBom> result = new List<PartinBom>();
            string querytext = "select TR.id_refcmp, TR.refcmp_nom from t_nomen TN join t_refcmp TR on TN.et_refakf_comp = TR.id_refcmp and TN.et_refakf_se = '" + FamName + "'"
               + " and TN.nomen_fin_validite is null order by TR.id_refcmp asc";

            NpgsqlCommand getdata = new NpgsqlCommand(querytext, Connection);
            NpgsqlDataReader readdata = getdata.ExecuteReader();

            if (readdata.HasRows)
            {
                while (readdata.Read())
                {
                    PartinBom tmp = new PartinBom();
                    tmp.PartNumber = readdata[0].ToString();
                    tmp.Name= readdata[1].ToString();
                    //MessageBox.Show(tmp.PartNumber + " " + tmp.Name); 
                    result.Add(tmp);
                }
                readdata.Close();
            }
            return result;

        }

        public List<PartinBom> getSeqBillOfMaterial(string id_trace_order)
        {
            List<PartinBom> result = new List<PartinBom>();
            string querytext = "select TT.trace_order_value, TR.refcmp_nom from t_trace_order TT join t_refcmp TR on TT.trace_order_value = TR.id_refcmp and TT.et_order_carord = '29' " +
                " and TT.id_trace_order = '" + id_trace_order + "' order by TT.trace_order_value asc";

            NpgsqlCommand getdata = new NpgsqlCommand(querytext, Connection);
            NpgsqlDataReader readdata = getdata.ExecuteReader();

            if (readdata.HasRows)
            {
                while (readdata.Read())
                {
                    PartinBom tmp = new PartinBom();
                    tmp.PartNumber = readdata[0].ToString();
                    tmp.Name = readdata[1].ToString();
                    //MessageBox.Show(tmp.PartNumber + " " + tmp.Name); 
                    result.Add(tmp);
                }
                readdata.Close();
            }
            return result;

        }

    }
}
