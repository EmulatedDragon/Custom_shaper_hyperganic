// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Collections.Generic;
using System.Threading;
using System.Numerics;

namespace Hyperganic
{
    public class SuperRecipe
    {
        public string m_strRecipeName { get; set; }
        public string m_strRecipe { get; set; }
        public bool m_bAnimatable { get; set; }
        public List<string> m_oUIElements { get; set; }
        public float m_fPrintVolumeSize { get; set; }
        public Dictionary<string, dynamic> m_aParameters { get; set; }
        public string m_strPreviewGeometryColor
        {
            get { return "B700"; }
        }

        public SuperRecipe(string strRecipeName, bool bAnimatable, float fPrintVolumeSize)
        {
            this.m_strRecipeName = strRecipeName;
            this.m_bAnimatable = bAnimatable;
            this.m_fPrintVolumeSize = fPrintVolumeSize;
            SetUIElements();
            ShowUIElements();
        }
        public void Prepare()
        {
            GetVariables();
            WriteRecipe();
        }
        public virtual void SetUIElements() { }
        private void ShowUIElements()
        {
            Hy.API.ExecuteCommand($"Sidebar.{m_strRecipeName}.Show()");
            Hy.API.ExecuteCommand($"Sidebar.{m_strRecipeName}.InputParametersGrid.animation.Show()");
            foreach (string strUIElement in m_oUIElements)
            {
                string strPathToUIElement = $"Sidebar.{m_strRecipeName}.InputParametersGrid.{strUIElement}";
                Hy.API.ExecuteCommand($"{strPathToUIElement}.Show();");
            }
            if (!m_bAnimatable)
            {
                Hy.API.ExecuteCommand($"Sidebar.{m_strRecipeName}.InputParametersGrid.animation.Hide();");
                Hy.API.ExecuteCommand($"Sidebar.{m_strRecipeName}.InputParametersGrid.animationText.Hide()");
            }
            else
            {
                Hy.API.ExecuteCommand($"Sidebar.{m_strRecipeName}.InputParametersGrid.animation.Show();");
                Hy.API.ExecuteCommand($"Sidebar.{m_strRecipeName}.InputParametersGrid.animationText.Show()");
            }
        }
        private void GetVariables()
        {
            m_aParameters = new Dictionary<string, dynamic>();
            foreach (string strUIElementVariableName in m_oUIElements)
            {
                string strPathToUIElementVariable = $"Project.{strUIElementVariableName}{m_strRecipeName}";
                string strUIElementVariableValue = Hy.API.strGetVariable(strPathToUIElementVariable);
                m_aParameters.Add(strUIElementVariableName, strUIElementVariableValue);
            }
        }
        public virtual void WriteRecipe() { }
        public virtual void Animate()
        {
            Hy.API.Log("Error", $"SuperRecipe '{m_strRecipeName}' does not support animation");
            return;
        }
        public virtual void AdjustPrintVolume()
        {
            Hy.Geometry geoCube = Recipe.geoLoadScaledCube(1);
            Recipe.AdjustPrintVolume(geoCube, m_fPrintVolumeSize);
            Program.oProjectDisplay.RecenterCamera();
            Program.oProjectDisplay.Zoom(-100);
        }
    }
    // Concepts: Basic 
    public class SphereShaperSuperRecipe : SuperRecipe
    {
        public SphereShaperSuperRecipe() : base("Sphere Shaper", false, 3) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
            {
                "fCircleRadius",
                "fXPos",
                "fYPos",
                "fZPos"
            };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latSphere = SphereShaper(
                    {float.Parse(m_aParameters["fCircleRadius"])},
                    {float.Parse(m_aParameters["fXPos"])},
                    {float.Parse(m_aParameters["fYPos"])},
                    {float.Parse(m_aParameters["fZPos"])});
                denResult = Voxelize(latSphere);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }
    }

    // Concepts: For Loops
    public class CuboidShaperSuperRecipe : SuperRecipe
    {
        public CuboidShaperSuperRecipe() : base("Cuboid Shaper", true, 6) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "fHeight",
                    "fLength",
                    "fWidth",
                    "fXStart",
                    "fYStart",
                    "fZStart"
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latCuboid = CuboidShaper(
                    {float.Parse(m_aParameters["fLength"])},
                    {float.Parse(m_aParameters["fWidth"])},
                    {float.Parse(m_aParameters["fHeight"])},
                    {float.Parse(m_aParameters["fXStart"])},
                    {float.Parse(m_aParameters["fYStart"])},
                    {float.Parse(m_aParameters["fZStart"])});
                denResult = Voxelize(latCuboid);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();
            Hy.Project.ClearPreviewGeometry();

            int iAnimationSmoothingBuffer = 20;
            float fStartLength = 15;
            float fStartWidth = 5;
            float fStartHeight = 1;
            float fEndLength = 2;
            float fEndWidth = 10;
            float fEndHeight = 20;
            float fLength = fStartLength;
            float fWidth = fStartWidth;
            float fHeight = fStartHeight;
            float fXStart = -7.5f;
            float fYStart = -2.5f;
            float fZStart = 0f;

            Hy.ShaperInstance oCuboidShaper = Hy.ShaperInterface.oGetShaperInstance("CuboidShaper");
            oCuboidShaper.bSetParameterValue(3, fXStart);
            oCuboidShaper.bSetParameterValue(4, fYStart);
            oCuboidShaper.bSetParameterValue(5, fZStart);

            int iIterations = 100;
            Hy.Lattice[] aLatticeList = new Hy.Lattice[iIterations];
            Hy.Lattice latCube;

            int iCurrentLoop = 0;
            while (iCurrentLoop < iIterations)
            {
                fLength += (fEndLength - fStartLength) / iIterations;
                fWidth += (fEndWidth - fStartWidth) / iIterations;
                fHeight += (fEndHeight - fStartHeight) / iIterations;

                oCuboidShaper.bSetParameterValue(0, fLength);
                oCuboidShaper.bSetParameterValue(1, fWidth);
                oCuboidShaper.bSetParameterValue(2, fHeight);
                oCuboidShaper.bProduceResult();
                latCube = oCuboidShaper.oGetLatticeReturn(0);

                aLatticeList[iCurrentLoop] = latCube;
                iCurrentLoop++;
            }
            foreach (Hy.Lattice lattice in aLatticeList)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Hy.Project.AddPreviewGeometry(lattice, m_strPreviewGeometryColor);
                Hy.Project.ClearPreviewGeometry(false);

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }
    public class PyramidShaperSuperRecipe : SuperRecipe
    {
        public PyramidShaperSuperRecipe() : base("Pyramid Shaper", true, 3) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
            {
                "fLength",
                "fHeight",
            };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latPyramid = PyramidShaper(
                    {float.Parse(m_aParameters["fLength"])},
                    {float.Parse(m_aParameters["fHeight"])});
                denResult = Voxelize(latPyramid);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();

            int iAnimationSmoothingBuffer = 80;
            float fLength = 10;
            float fHeight = 10;

            Hy.ShaperInstance oPyramidShaper = Hy.ShaperInterface.oGetShaperInstance("PyramidShaper");

            while (fLength > 3)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oPyramidShaper.bSetParameterValue(0, fLength);
                oPyramidShaper.bSetParameterValue(1, fHeight);
                oPyramidShaper.bProduceResult();
                Hy.Lattice latPyramid = oPyramidShaper.oGetLatticeReturn(0);

                Hy.Project.AddPreviewGeometry(latPyramid, m_strPreviewGeometryColor);
                Hy.Project.ClearPreviewGeometry(false);

                fHeight += 0.1f;
                fLength -= 0.1f;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }
    public class CylinderHollowCartesianShaperSuperRecipe : SuperRecipe
    {
        public CylinderHollowCartesianShaperSuperRecipe() : base("Cylinder Hollow Cartesian Shaper", true, 5) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "fCircleRadius",
                    "fHeight",
                    "fThickness",
                    "fResolution",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latCylinder = CylinderCartesian(
                    {float.Parse(m_aParameters["fCircleRadius"])},
                    {float.Parse(m_aParameters["fHeight"])},
                    {float.Parse(m_aParameters["fThickness"])},
                    {float.Parse(m_aParameters["fResolution"])});
                denResult = Voxelize(latCylinder);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();
            Hy.Project.ClearPreviewGeometry();

            int iAnimationSmoothingBuffer = 80;
            float fRadius = 15;
            float fHeight = 5;
            float fThickness = 6;
            float fResolution = 1;

            Hy.ShaperInstance oCylinderCartesianShaper = Hy.ShaperInterface.oGetShaperInstance("CylinderCartesian");
            oCylinderCartesianShaper.bSetParameterValue(0, fRadius);
            oCylinderCartesianShaper.bSetParameterValue(1, fHeight);
            oCylinderCartesianShaper.bSetParameterValue(2, fThickness);
            oCylinderCartesianShaper.bSetParameterValue(3, fResolution);
            oCylinderCartesianShaper.bProduceResult();
            Hy.Lattice latCylinder = oCylinderCartesianShaper.oGetLatticeReturn(0);
            while (fRadius > 3)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oCylinderCartesianShaper.bSetParameterValue(0, fRadius);
                oCylinderCartesianShaper.bSetParameterValue(2, fThickness);
                oCylinderCartesianShaper.bProduceResult();
                latCylinder = oCylinderCartesianShaper.oGetLatticeReturn(0);

                Hy.Project.AddPreviewGeometry(latCylinder, m_strPreviewGeometryColor);
                Hy.Project.ClearPreviewGeometry(false);

                fThickness -= 0.14f;
                fRadius -= 0.3f;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }

    // Concepts: Polar Coordinates
    public class CylinderHollowPolarShaperSuperRecipe : SuperRecipe
    {
        public CylinderHollowPolarShaperSuperRecipe() : base("Cylinder Hollow Polar Shaper", true, 5) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "fCircleRadius",
                    "fHeight",
                    "fThickness",
                    "fResolution",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latCylinder = CylinderPolar(
                    {float.Parse(m_aParameters["fCircleRadius"])},
                    {float.Parse(m_aParameters["fHeight"])},
                    {float.Parse(m_aParameters["fThickness"])},
                    {float.Parse(m_aParameters["fResolution"])});
                denResult = Voxelize(latCylinder);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();
            Hy.Project.ClearPreviewGeometry();

            int iAnimationSmoothingBuffer = 80;
            float fThickness = 6;
            float fRadius = 15;
            float fHeight = 5;
            float fResolution = 1;

            Hy.ShaperInstance oCylinderCartesianShaper = Hy.ShaperInterface.oGetShaperInstance("CylinderPolar");
            oCylinderCartesianShaper.bSetParameterValue(0, fRadius);
            oCylinderCartesianShaper.bSetParameterValue(1, fHeight);
            oCylinderCartesianShaper.bSetParameterValue(2, fThickness);
            oCylinderCartesianShaper.bSetParameterValue(3, fResolution);
            oCylinderCartesianShaper.bProduceResult();
            Hy.Lattice latCylinder = oCylinderCartesianShaper.oGetLatticeReturn(0);

            while (fRadius > 3)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oCylinderCartesianShaper.bSetParameterValue(0, fRadius);
                oCylinderCartesianShaper.bSetParameterValue(2, fThickness);
                oCylinderCartesianShaper.bProduceResult();
                latCylinder = oCylinderCartesianShaper.oGetLatticeReturn(0);

                Hy.Project.AddPreviewGeometry(latCylinder, m_strPreviewGeometryColor);
                Hy.Project.ClearPreviewGeometry(false);

                fThickness -= 0.14f;
                fRadius -= 0.3f;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }
    public class ConeShaperSuperRecipe : SuperRecipe
    {
        public ConeShaperSuperRecipe() : base("Cone Shaper", true, 8) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "fCircleRadius",
                    "fHeight",
                    "fXCenter",
                    "fYCenter",
                    "fZCenter",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latCone = ConeShaper(
                    {float.Parse(m_aParameters["fCircleRadius"])},
                    {float.Parse(m_aParameters["fHeight"])},
                    {float.Parse(m_aParameters["fXCenter"])},
                    {float.Parse(m_aParameters["fYCenter"])}, 
                    {float.Parse(m_aParameters["fZCenter"])});
                denResult = Voxelize(latCone);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();
            Hy.Project.ClearPreviewGeometry();

            int iAnimationSmoothingBuffer = 40;
            float fCircleRadius = 15;
            float fHeight = 5;

            Hy.ShaperInstance oConeShaper = Hy.ShaperInterface.oGetShaperInstance("ConeShaper");
            Hy.Lattice latCone;

            while (fCircleRadius > 2)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oConeShaper.bSetParameterValue(0, fCircleRadius);
                oConeShaper.bSetParameterValue(1, fHeight);
                oConeShaper.bProduceResult();
                latCone = oConeShaper.oGetLatticeReturn(0);

                Hy.Project.AddPreviewGeometry(latCone, m_strPreviewGeometryColor);
                Hy.Project.ClearPreviewGeometry(false);

                fHeight += 0.1f;
                fCircleRadius -= 0.15f;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }
    public class CustomSpringShaperSuperRecipe : SuperRecipe
    {
        public CustomSpringShaperSuperRecipe() : base("Custom Spring Shaper", true, 5) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "fCircleRadius",
                    "fHeight",
                    "fThickness",
                    "fAngularIncrement",
                    "iCoils",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latSpring = CustomSpring(
                    {float.Parse(m_aParameters["fCircleRadius"])},
                    {float.Parse(m_aParameters["fHeight"])},
                    {float.Parse(m_aParameters["fThickness"])},
                    {float.Parse(m_aParameters["fAngularIncrement"])},
                    {int.Parse(m_aParameters["iCoils"])});
                denResult = Voxelize(latSpring);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();
            Hy.Project.ClearPreviewGeometry();

            int iAnimationSmoothingBuffer = 50;
            float fAngularIncrement = 1f;
            float fHeight = 5;
            float fRadius = 15;

            Hy.ShaperInstance oCustomSpring = Hy.ShaperInterface.oGetShaperInstance("CustomSpring");
            oCustomSpring.bSetParameterValue(2, 0.2f);
            oCustomSpring.bSetParameterValue(4, 3);
            Hy.Lattice latSpring;

            while (fHeight < fRadius * 3)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oCustomSpring.bSetParameterValue(0, fRadius);
                oCustomSpring.bSetParameterValue(1, fHeight);
                oCustomSpring.bSetParameterValue(3, fAngularIncrement);
                oCustomSpring.bProduceResult();
                latSpring = oCustomSpring.oGetLatticeReturn(0);

                Hy.Project.AddPreviewGeometry(latSpring, m_strPreviewGeometryColor);
                Hy.Project.ClearPreviewGeometry(false);

                fAngularIncrement -= 0.01f;
                fHeight += 0.1f;
                fRadius -= 0.1f;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }

    // Concepts: Equations
    public class CustomEpicycloidShaperSuperRecipe : SuperRecipe
    {
        public CustomEpicycloidShaperSuperRecipe() : base("Epicycloid Shaper", true, 9) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "fOuterRadius",
                    "fCircleRadius",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latEpicycloid = CustomEpicycloid(
                    {float.Parse(m_aParameters["fCircleRadius"])},
                    {float.Parse(m_aParameters["fOuterRadius"])});
                denResult = Voxelize(latEpicycloid);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();

            float fIter = 0;
            int iAnimationSmoothingBuffer = 50;

            Hy.ShaperInstance oCustomEpicycloid = Hy.ShaperInterface.oGetShaperInstance("CustomEpicycloidAnimation");
            oCustomEpicycloid.bSetParameterValue(0, fIter);
            oCustomEpicycloid.bProduceResult();
            Hy.Lattice latEpicycloidInnerCircle = oCustomEpicycloid.oGetLatticeReturn(0);
            Hy.Lattice latEpicycloidOuterCirle;
            Hy.Lattice latEpicycloidLines;
            Hy.Lattice latEpicycloidPoints;

            Hy.API.SetVariable("Project.bResultGenerated", "true");

            while (fIter <= 2 * MathF.PI)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oCustomEpicycloid.bSetParameterValue(0, fIter);
                oCustomEpicycloid.bProduceResult();
                latEpicycloidInnerCircle = oCustomEpicycloid.oGetLatticeReturn(0);
                latEpicycloidOuterCirle = oCustomEpicycloid.oGetLatticeReturn(1);
                latEpicycloidLines = oCustomEpicycloid.oGetLatticeReturn(2);
                latEpicycloidPoints = oCustomEpicycloid.oGetLatticeReturn(3);

                Hy.Project.AddPreviewGeometry(latEpicycloidPoints, "B700");
                Hy.Project.AddPreviewGeometry(latEpicycloidInnerCircle, "KIWI");
                Hy.Project.AddPreviewGeometry(latEpicycloidOuterCirle, "LEMON");
                Hy.Project.AddPreviewGeometry(latEpicycloidLines, "TOMATO");

                Hy.Project.ClearPreviewGeometry(false);

                fIter += 2 * MathF.PI / 500;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime / 10);
            }
            Program.CollectGarbage();
        }
    }
    public class SuperFormula2DShaperSuperRecipe : SuperRecipe
    {
        public SuperFormula2DShaperSuperRecipe() : base("Super Formula 2D Shaper", false, 6.5f) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "Supershape2D",
                    "fm",
                    "fn1",
                    "fn2",
                    "fn3",
                    "fCircleRadius",
                    "fMetaballSize",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latShape2D = SuperFormula2D(
                    {float.Parse(m_aParameters["fm"])},
                    {float.Parse(m_aParameters["fn1"])},
                    {float.Parse(m_aParameters["fn2"])},
                    {float.Parse(m_aParameters["fn3"])},
                    {float.Parse(m_aParameters["fCircleRadius"])},
                    {float.Parse(m_aParameters["fMetaballSize"])});
                PreviewGeometry(latShape2D);
                denResult = Voxelize(latShape2D);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }
    }
    public class SuperFormula3DShaperSuperRecipe : SuperRecipe
    {
        public SuperFormula3DShaperSuperRecipe() : base("Super Formula 3D Shaper", false, 6.5f) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "Supershape3D",
                    "fm1",
                    "fn11",
                    "fn21",
                    "fn31",
                    "fm2",
                    "fn12",
                    "fn22",
                    "fn32",
                    "fCircleRadius",
                    "fAngularResolution",
                    "fMetaballSize",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latShape3D = SuperFormula3D(
                    {m_aParameters["fm1"]},
                    {m_aParameters["fn11"]},
                    {m_aParameters["fn21"]},
                    {m_aParameters["fn31"]},
                    {m_aParameters["fm2"]},
                    {m_aParameters["fn12"]},
                    {m_aParameters["fn22"]},
                    {m_aParameters["fn32"]},
                    {m_aParameters["fCircleRadius"]},
                    {m_aParameters["fAngularResolution"]},
                    {m_aParameters["fMetaballSize"]});
                PreviewGeometry(latShape3D);
                denResult = Voxelize(latShape3D);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }
    }

    // Concepts: Interpolation
    public class SmoothInterpolationShaperSuperRecipe : SuperRecipe
    {
        public SmoothInterpolationShaperSuperRecipe() : base("Smooth Interpolation Shaper", true, 10) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "iIterations",
                    "fRatio",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                latLines, latControlPoints = SmoothInterpolationShaper(
                    {int.Parse(m_aParameters["iIterations"])},
                    {float.Parse(m_aParameters["fRatio"])});
                PreviewGeometry(latLines, false, false, {m_strPreviewGeometryColor});
                PreviewGeometry(latControlPoints, false, false, LEMON);
                denResult = Voxelize(latControlPoints);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();

            float fRatio = 1.2f;
            float fIncrement = 0.005f;
            int iIterations = 10;
            int iAnimationSmoothingBuffer = 40;

            Hy.ShaperInstance oSmoothInterpolation = Hy.ShaperInterface.oGetShaperInstance("SmoothInterpolationShaper");

            Hy.Lattice latLines;
            Hy.Lattice latControlPoints;

            while (fRatio > -0.2f)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oSmoothInterpolation.bSetParameterValue(0, iIterations);
                oSmoothInterpolation.bSetParameterValue(1, fRatio);
                oSmoothInterpolation.bProduceResult();

                latLines = oSmoothInterpolation.oGetLatticeReturn(0);
                latControlPoints = oSmoothInterpolation.oGetLatticeReturn(1);

                Hy.Project.AddPreviewGeometry(latLines, m_strPreviewGeometryColor);
                Hy.Project.AddPreviewGeometry(latControlPoints, "LEMON");
                Hy.Project.ClearPreviewGeometry(false);

                fRatio -= fIncrement;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }
    public class BezierCurveShaperSuperRecipe : SuperRecipe
    {
        public BezierCurveShaperSuperRecipe() : base("Bezier Curve Shaper", true, 5) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "vecBez1",
                    "vecBez1.fX1",
                    "vecBez1.fY1",
                    "vecBez1.fZ1",
                    "vecBez2",
                    "vecBez2.fX2",
                    "vecBez2.fY2",
                    "vecBez2.fZ2",
                    "vecBez3",
                    "vecBez3.fX3",
                    "vecBez3.fY3",
                    "vecBez3.fZ3",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
            latLines, latPoints = BezierCurve(
                Vector( {float.Parse(m_aParameters["vecBez1.fX1"])}, {float.Parse(m_aParameters["vecBez1.fY1"])}, {float.Parse(m_aParameters["vecBez1.fZ1"])}),
                Vector( {float.Parse(m_aParameters["vecBez2.fX2"])}, {float.Parse(m_aParameters["vecBez2.fY2"])}, {float.Parse(m_aParameters["vecBez2.fZ2"])}),
                Vector( {float.Parse(m_aParameters["vecBez3.fX3"])}, {float.Parse(m_aParameters["vecBez3.fY3"])}, {float.Parse(m_aParameters["vecBez3.fZ3"])})
                );
            PreviewGeometry(latLines, false, false, {m_strPreviewGeometryColor});
            PreviewGeometry(latPoints, false, false, TOMATO);
            geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
            geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
            AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
            //Return(0, denResult);
            ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();

            float fIter = 0;
            int iAnimationSmoothingBuffer = 30;

            Hy.ShaperInstance oCustomBezier = Hy.ShaperInterface.oGetShaperInstance("BezierCurveAnimation");
            oCustomBezier.bSetParameterValue(0, fIter);
            oCustomBezier.bProduceResult();
            Hy.Lattice latLines = oCustomBezier.oGetLatticeReturn(0);
            Hy.Lattice latLine1;
            Hy.Lattice latLine2;
            Hy.Lattice latContour;

            Hy.Project.AddPreviewGeometry(latLines, "LEMON");

            while (fIter < 1)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oCustomBezier.bSetParameterValue(0, fIter);
                oCustomBezier.bProduceResult();
                latLines = oCustomBezier.oGetLatticeReturn(0);
                latLine1 = oCustomBezier.oGetLatticeReturn(1);
                latLine2 = oCustomBezier.oGetLatticeReturn(2);
                latContour = oCustomBezier.oGetLatticeReturn(3);

                Hy.Project.AddPreviewGeometry(latContour, "B700");
                Hy.Project.AddPreviewGeometry(latLines, "LEMON");
                Hy.Project.AddPreviewGeometry(latLine1, "TOMATO");
                Hy.Project.AddPreviewGeometry(latLine2, "KIWI");

                Hy.Project.ClearPreviewGeometry(false);

                fIter += 0.005f;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }

    // Concepts: Rotation
    public class RotationExplorerShaperSuperRecipe : SuperRecipe
    {
        public RotationExplorerShaperSuperRecipe(Action action) : base("Rotation Explorer Shaper", true, 5)
        {
            action();
        }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "strRotationAxis",
                    "fCircleRadius",
                    "fX",
                    "fY",
                    "fZ",
                    "iAngle",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                lattice, latAxis, latTracker = RotationExplorer(
                    {float.Parse(m_aParameters["fCircleRadius"])},
                    {int.Parse(m_aParameters["iAngle"])},
                    vector({float.Parse(m_aParameters["fX"])},
                    {float.Parse(m_aParameters["fY"])},
                    {float.Parse(m_aParameters["fZ"])}),
                    {m_aParameters["strRotationAxis"]});
                PreviewGeometry(lattice, false, false, {m_strPreviewGeometryColor});
                PreviewGeometry(latAxis, false, false, LEMON);
                PreviewGeometry(latTracker, false, false, TOMATO);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                //Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();

            int iAngle = 0;
            float fCircleRadius = 15;
            float fX = 1;
            float fY = 1;
            float fZ = 1;
            string strRotationAxis = "Custom";
            int iAnimationSmoothingBuffer = 15;

            Hy.ShaperInstance oRotationExplorer = Hy.ShaperInterface.oGetShaperInstance("RotationExplorer");
            oRotationExplorer.bSetParameterValue(0, fCircleRadius);
            oRotationExplorer.bSetParameterValue(1, iAngle);
            oRotationExplorer.bSetParameterValue(2, new Vector3(fX, fY, fZ));
            oRotationExplorer.bSetParameterValue(3, strRotationAxis);
            oRotationExplorer.bProduceResult();

            Hy.Lattice latCircle;
            Hy.Lattice latAxis = oRotationExplorer.oGetLatticeReturn(1);
            Hy.Lattice latTracker;

            while (iAngle <= 360)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oRotationExplorer.bSetParameterValue(1, iAngle);
                oRotationExplorer.bProduceResult();

                latCircle = oRotationExplorer.oGetLatticeReturn(0);
                latTracker = oRotationExplorer.oGetLatticeReturn(2);

                Hy.Project.AddPreviewGeometry(latCircle, m_strPreviewGeometryColor);
                Hy.Project.AddPreviewGeometry(latTracker, "TOMATO");
                Hy.Project.AddPreviewGeometry(latAxis, "LEMON");
                Hy.Project.ClearPreviewGeometry(false);

                iAngle += 1;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iComputingTime = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iComputingTime ? iAnimationSmoothingBuffer - iComputingTime : 0;
                Thread.Sleep(iSleepTime);
            }
        }
    }

    // Concepts: Meshes
    public class PolygonalPrismShaperSuperRecipe : SuperRecipe
    {
        private readonly float fAssemblyBias;
        public PolygonalPrismShaperSuperRecipe(float fAssemblyBias) : base("Polygonal Prism Shaper", true, 6)
        {
            this.fAssemblyBias = fAssemblyBias;
        }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
                {
                    "iNumberOfSides",
                    "fHeight",
                    "fDistanceToCenter",
                    "fXCenter",
                    "fYCenter",
                    "fZCenter",
                };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                asmPolygonalPrism = PolygonalPrismShaper(
                    {float.Parse(m_aParameters["iNumberOfSides"])},
                    {float.Parse(m_aParameters["fHeight"])},
                    {float.Parse(m_aParameters["fDistanceToCenter"])},
                    {float.Parse(m_aParameters["fXCenter"])},
                    {float.Parse(m_aParameters["fYCenter"])},
                    {float.Parse(m_aParameters["fZCenter"])},
                    {fAssemblyBias});
                asmPolygonalPrism = SetBias(asmPolygonalPrism,{fAssemblyBias});
                denResult = Voxelize(asmPolygonalPrism);
                geoAdjustVolumeCube = OpenModelFile({Recipe.strPathToCubeFile});
                geoAdjustVolumeCube = SetScale(geoAdjustVolumeCube, vector(1, 1, 1));
                AdjustPrintVolume(geoAdjustVolumeCube, {m_fPrintVolumeSize});
                Return(0, denResult);
                ";
        }

        public override void Animate()
        {
            base.AdjustPrintVolume();

            float fNumberOfSides = 50;
            float fHeight = 10;
            float fDistanceToCenter = 10;
            float fXCenter = 0;
            float fYCenter = 0;
            float fZCenter = 0;
            int iAnimationSmoothingBuffer = 100;

            Hy.ShaperInstance oPolygonalPrism = Hy.ShaperInterface.oGetShaperInstance("PolygonalPrismShaper");
            oPolygonalPrism.bSetParameterValue(0, fNumberOfSides);
            oPolygonalPrism.bSetParameterValue(1, fHeight);
            oPolygonalPrism.bSetParameterValue(2, fDistanceToCenter);
            oPolygonalPrism.bSetParameterValue(3, fXCenter);
            oPolygonalPrism.bSetParameterValue(4, fYCenter);
            oPolygonalPrism.bSetParameterValue(5, fZCenter);
            oPolygonalPrism.bSetParameterValue(6, fAssemblyBias);
            oPolygonalPrism.bProduceResult();
            Hy.Assembly asmPolygon = oPolygonalPrism.oGetAssemblyReturn(0);

            while (fNumberOfSides > 2)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                oPolygonalPrism.bSetParameterValue(0, fNumberOfSides);
                oPolygonalPrism.bProduceResult();

                asmPolygon = oPolygonalPrism.oGetAssemblyReturn(0);

                Hy.Project.AddPreviewGeometry(asmPolygon, m_strPreviewGeometryColor);
                Hy.Project.ClearPreviewGeometry(false);

                fNumberOfSides--;

                long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                int iLoopDuration = (int)(endTime - startTime);
                int iSleepTime = iAnimationSmoothingBuffer > iLoopDuration ? iAnimationSmoothingBuffer - iLoopDuration : 0;
                Thread.Sleep(iSleepTime);
            }
            Program.CollectGarbage();
        }
    }

    // Additional: 
    public class MyCustomShaperSuperRecipe : SuperRecipe
    {
        public MyCustomShaperSuperRecipe() : base("My custom Shaper", false, 2) { }

        public override void SetUIElements()
        {
            m_oUIElements = new List<string>()
            {
                //insert the names of the UI elements you want to add here. Check 'General.xml' to see which elements are available
            };
        }

        public override void WriteRecipe()
        {
            m_strRecipe = $@"
                //insert your Recipe here
                ";
        }
    }
}
