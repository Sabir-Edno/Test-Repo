using DataAccessSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ClsDataAccessApplication
{
    public class ClsApplicationData
    {

        public static bool DeleteApplication(int ApplicationID)
        {
            int AffectedRows = 0;

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Query = @"Delete Applications
            Where ApplicationID = @ApplicationID";

            SqlCommand Command = new SqlCommand(Query, Connection);

            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                Connection.Open();

                AffectedRows = Command.ExecuteNonQuery();
            }
            catch (Exception)
            {

            }
            finally
            {
                Connection.Close();
            }

            return (AffectedRows > 0);
        }

        public static int AddNewApplication(int PersonID, DateTime ApplicationDate, int ApplicationTypeID,
       byte ApplicationStatus, DateTime LastStatusDate, Decimal PaidFees, int CreatedByUserID)
        {
            int ApplicationID = -1;

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Query = @"Insert Into Applications (ApplicantPersonID , ApplicationDate , ApplicationTypeID 
            ,ApplicationStatus , LastStatusDate ,PaidFees , CreatedByUserID) 
            Values (@ApplicantPersonID , @ApplicationDate , @ApplicationTypeID 
            ,@ApplicationStatus , @LastStatusDate ,@PaidFees , @CreatedByUserID)
            Select Scope_identity()";

            SqlCommand Command = new SqlCommand(Query, Connection);

            Command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
            Command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
            Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
            Command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
            Command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
            Command.Parameters.AddWithValue("@PaidFees", PaidFees);
            Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                Connection.Open();

                object Result = Command.ExecuteScalar();

                if (Result != null && int.TryParse(Result.ToString(), out int IntrestedID))
                {
                    ApplicationID = IntrestedID;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                Connection.Close();
            }

            return ApplicationID;

        }

        public static bool UpdateApplication(int ApplicationID ,int PersonID, DateTime ApplicationDate, int ApplicationTypeID,
       byte ApplicationStatus, DateTime LastStatusDate, Decimal PaidFees, int CreatedByUserID)
        {
            int AffectedRows = -1;

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Query = @"Update Applications 
            set ApplicantPersonID = @ApplicantPersonID ,
            ApplicationDate = @ApplicationDate ,
            ApplicationTypeID = @ApplicationTypeID ,
            ApplicationStatus = @ApplicationStatus ,
            LastStatusDate = @LastStatusDate ,
            PaidFees = @PaidFees ,
            CreatedByUserID = @CreatedByUserID
            Where ApplicationID = @ApplicationID";

            SqlCommand Command = new SqlCommand(Query, Connection);

            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            Command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
            Command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
            Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
            Command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
            Command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
            Command.Parameters.AddWithValue("@PaidFees", PaidFees);
            Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                Connection.Open();

                AffectedRows = Command.ExecuteNonQuery();
            }
            catch (Exception)
            {

            }
            finally
            {
                Connection.Close();
            }

            return AffectedRows > 0;

        }

        public static bool GetApplicationInfoByID(int ApplicationID, ref int ApplicationPersonID,
       ref DateTime ApplicationDate, ref int ApplicationTypeID,
       ref DateTime LastStatusDate, ref byte ApplicationStatus, ref decimal PaidFees, ref int CreatedByUserID)
        {
            bool IsFound = false;

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Query = "Select * From Applications Where ApplicationID = @ApplicationID";

            SqlCommand Command = new SqlCommand(Query, Connection);

            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();

                if (Reader.Read())
                {
                    IsFound = true;

                    ApplicationPersonID = (int)Reader["ApplicantPersonID"];
                    ApplicationDate = (DateTime)Reader["ApplicationDate"];
                    ApplicationTypeID = (int)Reader["ApplicationTypeID"];
                    LastStatusDate = (DateTime)Reader["LastStatusDate"];
                    PaidFees = (decimal)Reader["PaidFees"];
                    CreatedByUserID = (int)Reader["CreatedByUserID"];
                    ApplicationStatus = (byte)Reader["ApplicationStatus"];

                }

                Reader.Close();
            }
            catch (Exception)
            {

            }
            finally
            {
                Connection.Close();
            }

            return IsFound;
        }

        public static DataTable GetAllApplication()
        {
            DataTable dt = new DataTable();

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Qeury = "Select * From Applications";

            SqlCommand Command = new SqlCommand(Qeury, Connection);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();

                if (Reader.HasRows)
                    dt.Load(Reader);
            }
            catch (Exception)
            {

            }
            finally
            {
                Connection.Close();
            }

            return dt;
        }

        public static bool IsApplicationExist(int ApplicationID)
        {

            bool IsFound = false;

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Query = "Select Found = 1 From Applications Where @ApplicationID = ApplicationID";

            SqlCommand Command = new SqlCommand(Query, Connection);

            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                Connection.Open();

                object Result = Command.ExecuteScalar();

                if (Result != null)
                    IsFound = true;
                else
                    IsFound = false;
            }
            catch (Exception)
            {
                IsFound = false;
            }
            finally
            {
                Connection.Close();
            }

            return IsFound;
        }

        public static bool DoesPersonHaveActiveApplication(int PersonID , int ApplicationTypeID)
        {
            return GetActiveApplicationID(PersonID, ApplicationTypeID) != -1;
        }

        public static int GetActiveApplicationID(int PersonID , int ApplicationTypeID)
        {
            int ActiveApplicationID = -1;

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Qurey = "Select ActiveApplicationID = ApplicationID From Applications where ApplicationPersonID = @ApplicationPersonID And ApplicationTypeID = @ApplicationTypeID And ApplicationStatus = 1";

            SqlCommand Command = new SqlCommand(Qurey, Connection);

            Command.Parameters.AddWithValue("@ApplicationPersonID", PersonID);
            Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

            try
            {
                Connection.Open();

                object Result = Command.ExecuteScalar();

                if (Result != null && int.TryParse(Result.ToString(), out int IntrestedID))
                    ActiveApplicationID = IntrestedID;
            }
            catch (Exception)
            {
                return ActiveApplicationID;
            }
            finally
            {
                Connection.Close();
            }

            return ActiveApplicationID;
        }

        public static int GetActiveApplicationIDForLicenseClass(int PersonID , int ApplicationTypeID , int LicenseClassID)
        {
            int ActiveApplicationID = -1;

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Qurey = @"SELECT ActiveApplicationID=Applications.ApplicationID  
                            From
                            Applications INNER JOIN
                            LocalDrivingLicenseApplications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID
                            WHERE ApplicantPersonID = @ApplicantPersonID 
                            and ApplicationTypeID=@ApplicationTypeID 
							and LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID
                            and ApplicationStatus=1";

            SqlCommand Command = new SqlCommand( Qurey, Connection);

            Command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
            Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
            Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

            try
            {
                Connection.Open();

                object Resutl = Command.ExecuteScalar();

                if (Resutl != null && int.TryParse(Resutl.ToString(), out int AppID))
                    ActiveApplicationID = AppID;
            }
            catch (Exception)
            {
                return ActiveApplicationID;
            }

            finally
            {
                Connection.Close();
            }

            return ActiveApplicationID;
        }

        public static bool UpdateStatus(int ApplicationID , short NewStatus)
        {
            int AffectedRows = 0;

            SqlConnection Connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string Query = @"Update Applications
            Set ApplicationStatus = @NewStatus ,
            LastStatusDate = @LastStatusDate
            Where ApplicationID = @ApplicationID";

            SqlCommand Command = new SqlCommand(Query , Connection);

            Command.Parameters.AddWithValue("@NewStatus", NewStatus);
            Command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);
            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                Connection.Open();

                AffectedRows = Command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return AffectedRows > 0;
            }
            finally
            {
                Connection.Close();
            }

            return AffectedRows > 0;

        }

    }
}
