using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrasshopperAsyncComponent;
using System.Windows.Forms;
using DesignCheck2;
using DesignCheck2.Definitions;
// Any text after a double-forward-slash is a comment to help you understand the code
// Please do not copy these into new calculations

// These 'using' statements refer to other libraries, generally there is no need
// to change these between calculations
using DesignCheck2.Framework;
using DesignCheck2.Framework.Attributes;
using static DesignCheck2.Framework.Maths; // This allows access to the DesignCheck mathematics library - Sqrt(...), Pow(...) etc.



namespace GrasshopperAsyncComponentDemo.SampleImplementations
{
    public class Sample_UselessCyclesAsyncComponent : GH_AsyncComponent
    {
        public override Guid ComponentGuid { get => new Guid("a0b08f1f-d863-42eb-a66c-15cf752e0763"); }

        protected override System.Drawing.Bitmap Icon { get => DesigncheckGHOffline.Properties.Resources.like; }


        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Sample_UselessCyclesAsyncComponent() : base("Sample Design Check Component", "Sample Design Check", "Adds numbers A and B together", "Arup", "DesignCheck")
        {
            BaseWorker = new DesignCheckCalc();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("A", "A", "First Number", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "Second Number", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("HTML", "HTML", "Html output report", GH_ParamAccess.item);
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendItem(menu, "Cancel", (s, e) =>
            {
                RequestCancellation();
            });
        }
    }

    public class DesignCheckCalc : WorkerInstance
    {
        SimpleCalculation calc { get; set; }

        string Report { get; set; }


        public DesignCheckCalc() : base(null) { }

        public override void DoWork(Action<string, double> ReportProgress, Action Done)
        {
            // Checking for cancellation
            if (CancellationToken.IsCancellationRequested) { return; }

            Report = calc.ArupComputeReport_HTML;
            


            Done();
        }

        public override WorkerInstance Duplicate() => new DesignCheckCalc();

        public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
        {
            if (CancellationToken.IsCancellationRequested) return;

            double _A = 100.0;
            double _B = 100.0;
            DA.GetData(0, ref _A);
            DA.GetData(1, ref _B);

            SimpleCalculation _calc = new SimpleCalculation(_A, _B);
            calc = _calc;


        }

        public override void SetData(IGH_DataAccess DA)
        {
            if (CancellationToken.IsCancellationRequested) return;
            DA.SetData(0, Report);
        }
    }





    public class SimpleCalculation : Calculation
    {
        // *Results*
        // Results are declared outside of the body of the calculation
        // to make them easier to access.
        // DesignCheck convention is to previx them with 'Results_'
        // to make them easier to find e.g. SimpleCalculation.Results_x
        public Result<double> Results_C;

        //// *Calculation*
        //// Results
        //public Result<double> Results_K;

        public SimpleCalculation(
            double input_A,
            double input_B

        )
        {
            AddTitle("Add Two Numbers:"); // This scrapes the Calculation meta-data and displays it nicely in the report output

            AddSubHeader("Inputs", false);
            Variable A = DeclareInput(input_A, DesignCheck2.Definitions.Test.A.Properties); // Within calculations the 'Properties' of the definition are used rather than 'Json'
            Variable B = DeclareInput(input_B, DesignCheck2.Definitions.Test.B.Properties);

            Variable C = CalcVariable(A + B, DesignCheck2.Definitions.Test.C.Properties);

            Results_C = AddResult(C);


            AddBibliography();
            // After digesting this simple example move on to Examples.SubCalculation.cs

        }

    }// 


}
