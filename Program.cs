// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Threading;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Hyperganic
{
    public static partial class Program
    {
        private static string m_strRecipeToGenerate;
        private static bool m_bAnimate = false;
        public static SuperRecipe oCurrentSuperRecipe
        {
            get {
				if (m_oCurrentSuperRecipe == null)
				{
					return new SphereShaperSuperRecipe();
				}
				return m_oCurrentSuperRecipe;
            }
            set {m_oCurrentSuperRecipe = value; }
        }
        public static Hy.ProjectDisplay oProjectDisplay
        {
            get { return m_oProjectDisplay; }
        }
        private static SuperRecipe m_oCurrentSuperRecipe;

        public static void Initialize()
        {
            oProjectDisplay.SetVoxelMode();
            oProjectDisplay.SetAutoRecenterCamera(false);
            SetUpRecipe();
            //CloseMenu("Sidebar.UI_Playground");
        }

        public static void SetUpRecipe()
        {
            m_strRecipeToGenerate = Hy.API.strGetVariable("Project.RecipeNumber");
            GetAnimateStatus();
            if (m_bAnimate)
            {
                DisableAnimationMode();
            }
            HideCurrentRecipeUIElements();
            DeleteModel();

            oCurrentSuperRecipe = m_strRecipeToGenerate switch
            {
                "Sphere Shaper" => new SphereShaperSuperRecipe(),//see default value at the bottom of this switch expression
                "Cuboid Shaper" => new CuboidShaperSuperRecipe(),
                "Pyramid Shaper" => new PyramidShaperSuperRecipe(),
                "Cylinder Hollow Cartesian Shaper" => new CylinderHollowCartesianShaperSuperRecipe(),
                "Cylinder Hollow Polar Shaper" => new CylinderHollowPolarShaperSuperRecipe(),
                "Cone Shaper" => new ConeShaperSuperRecipe(),
                "Custom Spring Shaper" => new CustomSpringShaperSuperRecipe(),
                "Epicycloid Shaper" => new CustomEpicycloidShaperSuperRecipe(),
                "Super Formula 2D Shaper" => new SuperFormula2DShaperSuperRecipe(),
                "Super Formula 3D Shaper" => new SuperFormula3DShaperSuperRecipe(),
                "Smooth Interpolation Shaper" => new SmoothInterpolationShaperSuperRecipe(),
                "Bezier Curve Shaper" => new BezierCurveShaperSuperRecipe(),
                "Rotation Explorer Shaper" => new RotationExplorerShaperSuperRecipe(ReadRotationAxisFromUI),
                "Polygonal Prism Shaper" => new PolygonalPrismShaperSuperRecipe(0.5f),
                "My custom Shaper" => new MyCustomShaperSuperRecipe(),
                _ => new SphereShaperSuperRecipe(),
            };
        }

        public static void HideCurrentRecipeUIElements()
        {
            string strCurrentRecipeName;
			try
			{
				strCurrentRecipeName = oCurrentSuperRecipe?.m_strRecipeName;
			}
			catch (System.NullReferenceException)
			{
                System.Diagnostics.Debug.Print($@" NULL Exception");
                return;
			}
            if (Hy.UIElement.bExists($"Sidebar.{strCurrentRecipeName}"))
            {
                Hy.UIElement oRecipePanel = new Hy.UIElement($"Sidebar.{strCurrentRecipeName}");
                oRecipePanel.Hide();
            }
        }

        public static void ReadRotationAxisFromUI() //is also called from General.xml's ComboBox strRotationAxis on each ComboBox change
        {
            string strRotationAxisToGenerate = Hy.API.strGetVariable($"Project.strRotationAxisRotation Explorer Shaper");
            Hy.API.ExecuteCommand($"Sidebar.Rotation Explorer Shaper.InputParametersGrid.RotationExplorerUI.FloatInputAngle.Show()");
            if (strRotationAxisToGenerate == "Custom")
            {
                Hy.API.ExecuteCommand($"Sidebar.Rotation Explorer Shaper.InputParametersGrid.fX.Show()");
                Hy.API.ExecuteCommand($"Sidebar.Rotation Explorer Shaper.InputParametersGrid.fY.Show()");
                Hy.API.ExecuteCommand($"Sidebar.Rotation Explorer Shaper.InputParametersGrid.fZ.Show()");
            }
            else
            {
                Hy.API.ExecuteCommand($"Sidebar.Rotation Explorer Shaper.InputParametersGrid.fX.Hide()");
                Hy.API.ExecuteCommand($"Sidebar.Rotation Explorer Shaper.InputParametersGrid.fY.Hide()");
                Hy.API.ExecuteCommand($"Sidebar.Rotation Explorer Shaper.InputParametersGrid.fZ.Hide()");
            }
        }

        public static void GetAnimateStatus()
        {
            m_bAnimate = Hy.API.bGetVariable("Project.bAnimation");
        }

        public static void EnableAnimationMode()
        {
            oProjectDisplay.SetMeshMode();
            foreach (string strUIElement in oCurrentSuperRecipe.m_oUIElements)
            {
                string strPathToUIElement = $"Sidebar.{oCurrentSuperRecipe.m_strRecipeName}.InputParametersGrid.{strUIElement}";
                Hy.API.ExecuteCommand($"{strPathToUIElement}.Disable();");
            }
            Hy.API.ExecuteCommand("ProjectDisplay.SplittingPlaneButton.Disable()");
            Hy.API.ExecuteCommand("ProjectDisplay.PrintingInformation.Hide();");
        }

        public static void DisableAnimationMode()
        {
            string strPathToAnimationToggle;
            try
            {
                strPathToAnimationToggle = $"Sidebar.{oCurrentSuperRecipe.m_strRecipeName}.InputParametersGrid.animation";
                Hy.API.ExecuteCommand($"{strPathToAnimationToggle}.TurnOff();");
            }
            catch (NullReferenceException) { return; }

            Hy.Project.ClearPreviewGeometry();
            Hy.MeshData oEmptyMesh = new Hy.MeshData(0, 0);
            oEmptyMesh.nAddVertex(new Vector3(0, 0, 0));
            oEmptyMesh.nAddTriangle(0, 0, 0);
            Hy.Project.AddPreviewGeometry(new Hy.Assembly(oEmptyMesh));

            oProjectDisplay.SetVoxelMode();
            foreach (string strUIElement in oCurrentSuperRecipe.m_oUIElements)
            {
                string strPathToUIElement = $"Sidebar.{oCurrentSuperRecipe.m_strRecipeName}.InputParametersGrid.{strUIElement}";
                Hy.API.ExecuteCommand($"{strPathToUIElement}.Enable();");
            }
            DeleteModel();
            Hy.API.ExecuteCommand("ProjectDisplay.SplittingPlaneButton.Enable()");
            Hy.API.ExecuteCommand("ProjectDisplay.PrintingInformation.Show();");
        }

        public static void Generate() //is called from inside UI.xml, via the "Generate" button
        {
            ToggleProjectDisplayElementsVisibility();
            oCurrentSuperRecipe.Prepare();


            GetAnimateStatus();
            if (!m_bAnimate)
            {
                Recipe.GenerateModel();
                Hy.Project.WaitForRecipeThread();
                oProjectDisplay.RecenterCamera();
				oProjectDisplay.Zoom(-100);
				Hy.API.SetVariable("Project.bResultGenerated", "true");
            }
			else
            {
                oCurrentSuperRecipe.Animate();
            }
            
            ToggleProjectDisplayElementsVisibility();
            CollectGarbage();
        }

        public static void OnSelectionChanged(int iModel)
        {

        }

        public static void ExportSTL()
        {
            Hy.API.ExecuteCommand("Application.SetVariable(strCurrentExportType,STL_DENSITY)");
            Hy.API.ExecuteCommand("PleaseWait.Show()");
            Hy.API.ExecuteCommand("Application.GetExportsLeft()");
        }

        public static void ToggleProjectDisplayElementsVisibility()
        {
            if (Hy.API.bGetVariable("ProjectDisplay.EnableSplittingPlane"))
            {
                ToggleSplittingPlane();
                Thread.Sleep(100);
            }

            string[] aUIElementsPaths = new string[]
            {
                "ProjectDisplay.HyPoweredLogo",
                "ProjectDisplay.ImportFile",
                "ProjectDisplay.SplittingPlaneButton",
            };
            Hy.UIElement oUIElement;
            foreach (string strPath in aUIElementsPaths)
            {
                if (Hy.UIElement.bExists(strPath))
                {
                    oUIElement = new Hy.UIElement(strPath);
                    if (oUIElement.bVisible())
                    {
                        oUIElement.Hide();
                    }
                    else
                    {
                        oUIElement.Show();
                    }
                }
            }
        }

        public static void SplittingPlaneToggle()
        {
            if (!Hy.API.bGetVariable("Project.bResultGenerated"))
            {
                return;
            }
            string strEnabled = Hy.API.strGetVariable("ProjectDisplay.EnableSplittingPlane");
            bool bEnable = strEnabled != "true";
            if (bEnable)
            {
                Hy.API.ExecuteCommand("Sidebar.Enable()");
                Hy.API.ExecuteCommand("GenerateExportPanel.Enable()");
            }
            else
            {
                Hy.API.ExecuteCommand("Sidebar.Disable()");
                Hy.API.ExecuteCommand("GenerateExportPanel.Disable()");
            }
        }
        public static void Collapse(Hy.UIElement oMenu)
        {
            string str = oMenu.strName();
            oMenu.MoveToPosition("Collapsed", 0.3f, Hy.Easing.EASE_IN_CUBIC);
            Hy.API.ExecuteCommand($"{oMenu.strName()}.Toggle.TurnOff()");
        }

        public static void Expand(Hy.UIElement oMenu)
        {
            oMenu.MoveToPosition("Expanded", 0.3f, Hy.Easing.EASE_IN_CUBIC);
            Hy.API.ExecuteCommand($"{oMenu.strName()}.Toggle.TurnOn()");
        }
        public static void CloseMenu(string strMenu)
        {
            Collapse(new Hy.UIElement(strMenu));
        }

        public static void OpenMenu(string strMenu)
        {
            Expand(new Hy.UIElement(strMenu));
        }
    
        public static void DoOpenModel(string strImportFilePath)
        {
			Recipe.SetRecipe("ClearGeomertry();" + " Return(0,null);");
            Hy.API.SetVariable("Project.bResultGenerated", "false");
            Hy.Project.WaitForRecipeThread();

            bool bIsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string strPathSeparator = "";
            if(bIsWindows==true)
            {
                strPathSeparator = "\\";
            }
            else
            {
                strPathSeparator = "/";
            }
            string[] aPathSplit = strImportFilePath.Split("/");
            List<string> aPathSplitList = aPathSplit.ToList();
            string strImportFileName = aPathSplitList[aPathSplitList.Count-1];
            aPathSplitList.RemoveAt(aPathSplitList.Count-1);
            string strImportFolderPath = String.Join(strPathSeparator, aPathSplitList);
            string [] strFileNameSplit = strImportFileName.Split(".");
            string strImportFileExtention = strFileNameSplit[strFileNameSplit.Length-1].ToLower();

            Hy.API.SetVariable($"Project.strImportFileExtention",   strImportFileExtention);
            Hy.API.SetVariable($"Project.strImportFilePath",        strImportFilePath);
            Hy.API.SetVariable($"Project.strImportFileName",        strImportFileName);
            Hy.API.SetVariable($"Project.strImportFolderPath",      strImportFolderPath);

           
            Hy.Material oMaterial =  Hy.Project.oMaterial();
            string strColor = oMaterial.strGetProperty("color").ToUpper();
            string strPreview = "";
            
            Hy.API.Log("Recipe", "Showing preview for imported model.");
            
            List<string> aAcceptedExtensions4Model = new List<string>()
            {
                "obj",
                "stl"
            };
            if (aAcceptedExtensions4Model.Contains(strImportFileExtention))
            {
                strPreview =            "\n"+
                          $"                            geoImport = OpenModelFile({strImportFilePath}, keep, keep, 0, edge);\n" +
                           "                            geoImport = Center(geoImport);\n" +
                          $"                            PreviewGeometry(geoImport,false,false, {strColor});\n" +
                           "                            AdjustPrintVolume(geoImport, 1.2);\n"+
                                        "\n";
                Hy.API.ExecuteCommand("ProjectDisplay.SplittingPlaneButton" + ".Hide()");
            }

            
            List<string> aAcceptedExtensions4Voxel = new List<string>()
            {
                "hvx"
            };
            if (aAcceptedExtensions4Voxel.Contains(strImportFileExtention))
            {
                strPreview =            "\n"+
                          $"                            denImport = OpenVoxelFile({strImportFilePath});\n" +
                           "                            AdjustPrintVolume(denImport, 1.2);\n"+
                           "                            Return(0, denImport);\n"+
                                        "\n";
                Hy.API.SetVariable("Project.bResultGenerated", "true");
                Hy.API.ExecuteCommand("ProjectDisplay.SplittingPlaneButton" + ".Show()");
            }

            Recipe.SetRecipe(strPreview);
            Hy.Project.WaitForRecipeThread();
            

            return;
		}
    }

    public static partial class Recipe
    {
        //link file paths to your input assets
        private static string m_strPathToProject = Directory.GetParent(Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName).FullName).FullName;
        private static string m_strPathToCubeFile = Path.Join(m_strPathToProject, "Assets", "Inputs", "STL", "Cube.stl");
        public static string strPathToCubeFile
        {
            get { return m_strPathToCubeFile; }
        }

        public static Hy.Geometry geoLoadScaledCube(float fSideLength)
        {
            Hy.OpenModelFile.Run(out Hy.Geometry geoCube, m_strPathToCubeFile);

            Hy.SetScale.Run(out Hy.Geometry geoScaledCube, geoCube, new Vector3(fSideLength));

            return geoScaledCube;
        }

        public static void AdjustPrintVolume(Hy.Geometry geoGeometry, float fScale)
        {
            Hy.ShaperInstance oAdjustPrintVolume = Hy.ShaperInterface.oGetShaperInstance("AdjustPrintVolume");
            oAdjustPrintVolume.bSetParameterValue(0, geoGeometry);
            oAdjustPrintVolume.bSetParameterValue(1, fScale);
            oAdjustPrintVolume.bProduceResult();
        }

        public static void AdjustPrintVolume(Hy.DensityField denDensity, float fScale)
        {
            Hy.ShaperInstance oAdjustPrintVolume = Hy.ShaperInterface.oGetShaperInstance("AdjustPrintVolume");
            oAdjustPrintVolume.bSetParameterValue(0, denDensity);
            oAdjustPrintVolume.bSetParameterValue(1, fScale);
            oAdjustPrintVolume.bProduceResult();
        }

        public static void GenerateModel()
        {
            SetRecipe(Program.oCurrentSuperRecipe.m_strRecipe);
            Hy.API.Log("CustomShaper", "Setting the recipe");
        }
    }
}
