using System;
using System.Collections.Generic;
using DesignCheck2.Framework;
using DesignCheck2.Definitions;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace DesigncheckGHOffline
{
    public class TimberColumnPresizing : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyDesignCheck class.
        /// </summary>
        public TimberColumnPresizing()
          : base("TimberColumnPresizing",
                "Designcheck",
                "Column sizing based on look up tables in design check",
                "Arup",
                "Elements Sizing")
        {
        }



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Force", "N", "Axial force", GH_ParamAccess.item);
            pManager.AddTextParameter("Strenght", "GL", "Glulam Strenght Long or medium load duration", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("ColumnSize", "B", "Output C", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // check the number of input parameters in Grade input
            if (Params.Input[1].SourceCount == 0)
            {
                // if there's no input then call the function to create the value list input    
                CreateValueListInput();
                ExpireSolution(true);
            }

            //Get input data
            double _N = 0.0;
            GH_Number gh_num = new GH_Number();
            if (DA.GetData(0, ref gh_num))
                GH_Convert.ToDouble(gh_num, out _N, GH_Conversion.Both); //use Grasshopper to convert, these methods covers many cases and are consistent

            string _GL = "GL24h_Long"; // set this to the default
            GH_String gh_text = new GH_String();
            if (DA.GetData(1, ref gh_text))
                GH_Convert.ToString(gh_text, out _GL, GH_Conversion.Both);

            // cast text input to designcheck enum
           // DesignCheck2.Enums.Structural.EC2.Concrete.Strength.concrete_grade fckEnum = (DesignCheck2.Enums.Structural.EC2.Concrete.Strength.concrete_grade)Enum.Parse(typeof(DesignCheck2.Enums.Structural.EC2.Concrete.Strength.concrete_grade), _Fck);

            DesignCheck2.Enums.Structural.EC5.GlulamStrength.glulam_strength glEnum = (DesignCheck2.Enums.Structural.EC5.GlulamStrength.glulam_strength)Enum.Parse(typeof(DesignCheck2.Enums.Structural.EC5.GlulamStrength.glulam_strength), _GL);
            // call the designcheck calculation
            DesignCheck2.Structural.EC.Calcs.Timber.Column.PreSizingGlulam preSizing_Glulam = new DesignCheck2.Structural.EC.Calcs.Timber.Column.PreSizingGlulam("", _N, glEnum);


            // set the output
            DA.SetData(0, preSizing_Glulam.Results_b.Value);
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
                return DesigncheckGHOffline.Properties.Resources.timber;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bb15dac3-ab15-4f0b-a3c6-e87967632f84"); }
        }

        /// <summary>
        /// Creates a new value list input
        /// </summary>
        void CreateValueListInput()
        {
            // Grasshopper will run all components during loading sequence, so 
            // if canvas is null step out of this code
            if (Instances.ActiveCanvas == null)
                return;

            //instantiate  new value list
            var vallist = new Grasshopper.Kernel.Special.GH_ValueList();
            vallist.CreateAttributes();

            // set the location relative to the current GH components location on the canvas
            vallist.Attributes.Pivot = new PointF(
                (float)Attributes.DocObject.Attributes.Bounds.Left  // lefternmost location of component
                - vallist.Attributes.Bounds.Width + 30, // minus width of value list + 30 
                (float)Params.Input[1].Attributes.Bounds.Y + 10); //y-location

            // create input list by reading in the possible enums from designcheck
            List<string> concreteGrades =
                Enum.GetValues(typeof(DesignCheck2.Enums.Structural.EC5.GlulamStrength.glulam_strength))
                .Cast<DesignCheck2.Enums.Structural.EC5.GlulamStrength.glulam_strength>()
                .Select(v => v.ToString()) //convert all items to string
                .ToList(); // convert array to list

            //populate value list with our own data
            vallist.ListItems.Clear();
            // loop through all enums and add them as inputs in the valuelist            
            for (int i = 0; i < concreteGrades.Count; i++)
            {
                var input = new Grasshopper.Kernel.Special.GH_ValueListItem(
                    concreteGrades[i].Replace('_', '/'),
                    "\"" + concreteGrades[i] + "\"");
                vallist.ListItems.Add(input);
            }

            //Until now, the slider is a hypothetical object.
            // This command makes it 'real' and adds it to the canvas.
            Instances.ActiveCanvas.Document.AddObject(vallist, false);

            //Connect the new slider to this component
            Params.Input[1].AddSource(vallist);
        }
    }
}


