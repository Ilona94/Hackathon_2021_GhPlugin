using System;
using System.Collections.Generic;
using DesignCheck2.Framework;
using DesignCheck2.Definitions;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DesigncheckGHOffline
{
    public class MyDesignCheck : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyDesignCheck class.
        /// </summary>
        public MyDesignCheck()
          : base("MyDesignCheck", "Designcheck",
              "Description",
              "Arup", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("A", "A", "First Number", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "Second Number", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddNumberParameter("C", "C", "Output C", GH_ParamAccess.item);
            pManager.AddTextParameter("HTML", "HTML", "Html output report", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Import data
            double _A = 0.0;
            double _B = 0.0;
            DA.GetData(0, ref _A);
            DA.GetData(1, ref _B);

            //Check if data is correct
            if (double.IsNaN(_A))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Value A is null");
                return;
            }
            
            if (double.IsNaN(_B))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Value B is null");
                return;
            }

            SimpleCalculation simple = new SimpleCalculation(_A, _B);

            //Output as double
            double _C = simple.Results_C.ValueTyped;

            //Output as HTML
            string _CHtml = simple.Results_C.HTML;


            DA.SetData(0, _C);
            DA.SetData(1, _CHtml);

        }

        


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return DesigncheckGHOffline.Properties.Resources.like;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8fd4d5fe-7574-466d-bf4b-1e3917024973"); }
        }
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