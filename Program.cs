using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSProject {

    class Staff {
        private float hourlyRate;
        private int hWorked;

        public float TotalPay {

            get;
            protected set;

        }

        public float BasicPay {

            get;
            private set;

        }

        public String NameOfStaff {

            get;
            private set;

        }

        public int HoursWorked {
            get {
                return hWorked;
            }
            set {
                if (value > 0) {
                    hWorked = value;
                } else {
                    hWorked = 0;
                }
            }

        }

        public Staff(String name, float rate) {
            this.NameOfStaff = name;
            this.hourlyRate = rate;
        }

        public virtual void CalculatePay() {
            Console.WriteLine("Calculating Pay...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public override string ToString() {
            return "\nName of Staff: " + NameOfStaff + "\nhourly rate: " + hourlyRate + "\nhours worked: " + HoursWorked +
                "\nBasic Pay: " + BasicPay + "\nTotalPay: " + TotalPay;
        }

    }

    class Manager : Staff {
        private const float managerHourlyRate = 50;

        public Manager(string name)
            : base(name, managerHourlyRate) {

        }



        public int Allowance {
            get;
            private set;
        }

        public override void CalculatePay() {
            base.CalculatePay();
            Allowance = 1000;

            if (HoursWorked > 160) {
                TotalPay = BasicPay + Allowance;
            }

        }

        public override string ToString() {

            if (HoursWorked > 160) {
                return "\nName of Staff: " + NameOfStaff + "\nManager hourly rate: " + managerHourlyRate + "\nHours Worked: "
                    + HoursWorked + "\nBasic Pay: " + BasicPay + "\nAllowance: " + Allowance + "\nTotal Pay: " + TotalPay;
            } else
                return "\nName of Staff: " + NameOfStaff + "\nManager hourly rate: " + managerHourlyRate + "\nHours Worked: "
                + HoursWorked + "\nBasic Pay: " + BasicPay + "\nTotal Pay: " + TotalPay;  
            
        }

    }

    class Admin : Staff {
        private const float overtimeRate = 15.5F;
        private const float adminHourlyRate = 30F;
        public float Overtime {
            get;
            private set;
        }

        public Admin(String name)
            : base(name, adminHourlyRate) {

        }

        public override void CalculatePay() {
            base.CalculatePay();


            if (HoursWorked > 160) {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay = Overtime + BasicPay;
            }
        }

        public override string ToString() {
            if (HoursWorked > 160) {
                return "\nName of Staff: " + NameOfStaff + "\nAdmin hourly rate: " + adminHourlyRate + "\nHours Worked: "
                    + HoursWorked + "\nBasic Pay: " + BasicPay + "\nOverTime Pay: " + Overtime + "\nTotal Pay: " + TotalPay;
            } else {
                return "\nName of Staff: " + NameOfStaff + "\nAdmin hourly rate: " + adminHourlyRate + "\nHours Worked: "
                    + HoursWorked + "\nBasic Pay: " + BasicPay + "\nTotal Pay: " + TotalPay;
            } 


        }

    }

    class FileReader {
        public List<Staff> ReadFile() {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] seperator = { ", " };

            if (File.Exists(path)) {
                using (StreamReader sr = new StreamReader(path)) {
                    while (sr.EndOfStream != true) {
                        result = (sr.ReadLine().Split(seperator,StringSplitOptions.RemoveEmptyEntries));

                        if (result[1] == "Manager") {
                            myStaff.Add(new Manager(result[0]));
                        } else if (result[1] == "Admin") {

                            myStaff.Add(new Admin(result[0]));
                        } else {
                        }
                    }

                    sr.Close();
                }
            } else {
                Console.WriteLine("Error has occured");
            }
            return myStaff;


        }
    }

    class PaySlip {

        private int month;
        private int year;



        enum MonthsOfYear {
            JAN = 1, FEB = 2, MAR = 3, APR = 4, MAY = 5, JUN = 6,
            JUL = 7, AUG = 8, SEP = 9, OCT = 10, NOV = 11, DEC = 12
        }

        public PaySlip(int payMonth, int payYear) {

            this.month = payMonth;
            this.year = payYear;

        }

        public void GeneratePaySlip(List<Staff> myStaff) {
            string path;
            try {
                foreach (Staff f in myStaff) {
                    path = f.NameOfStaff + ".txt";

                    StreamWriter sw = new StreamWriter(path);
                    sw.WriteLine("PAYSLIP FOR " + (MonthsOfYear)month, year);
                    sw.WriteLine("========================================");
                    sw.WriteLine("Name of Staff: " + f.NameOfStaff);
                    sw.WriteLine("Hours Worked: " + f.HoursWorked);
                    sw.WriteLine("Basic Pay: " + f.BasicPay);
                    if (f.GetType() == typeof(Manager)&& f.HoursWorked > 160) {
                        sw.WriteLine("Allowance: " + ((Manager)f).Allowance);
                    } else if (f.GetType() == typeof(Admin) && f.HoursWorked > 160) {
                        sw.WriteLine("Overtime: " + ((Admin)f).Overtime);
                    }
                    sw.WriteLine("=========================================");
                    sw.WriteLine("Total Pay: " + f.TotalPay);
                    sw.WriteLine("=========================================");
                    sw.Close();
                }
            } catch (Exception e) {
                Console.Write(e.Message + "PaySlip Writer Error");
            }
        }

        public void GenerateSummary(List<Staff> myStaff) {
            var result =
                from s in myStaff
                where s.HoursWorked < 10
                orderby s.NameOfStaff ascending
                select new {
                    s.NameOfStaff,
                    s.HoursWorked
                };


            var path = "summary.txt";
            try { 
            using (StreamWriter sw = new StreamWriter(path)) {

                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");
                foreach (var r in result) {
                    sw.WriteLine("\nName of Staff: " + r.NameOfStaff + ", \nHours Worked: " + r.HoursWorked);
                }
                sw.Close();
            }

            } catch(Exception e) {
                Console.Write(e.Message + "Summary Writer Error");
            }
        }
        public override string ToString() {
            return "month: " + month + "year: " + year;
        }




    }

    class Program {

        static void Main(string[] args) {
            List<Staff> myStaff = new List<Staff>();

            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;

            while (year == 0) {
                Console.Write("\nPlease enter the year: ");

                try {
                    //Code to convert the input to an integer

                    year = Convert.ToInt32(Console.ReadLine());

                } catch (Exception e) {
                    //Code to handle the exception
                    Console.Write(e.Message + "Incorrect Input");

                }



            }
            while (month == 0) {
                Console.Write("\nPlease enter a month: ");

                try {
                    //Code to convert the input to an integer

                    month = Convert.ToInt32(Console.ReadLine());

                    if (month < 1 || month > 12) {
                        Console.Write("Month is invalid. Try again");
                        month = 0;

                    }
                } catch (Exception e) {
                    Console.WriteLine(e.Message + "Error, try again");
                }
            }

            myStaff = fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++) {
                try {
                    Console.Write("Enter hours worked for " + myStaff[i].NameOfStaff + ":");

                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                } catch (Exception e) {
                    Console.WriteLine(e.Message+ "error in count staff");
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);
            Console.Read();
        }


    }
}
