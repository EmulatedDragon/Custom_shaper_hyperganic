using System;
using System.Collections.Generic;
using System.Text;

namespace Hyperganic
{
	public static partial class Program
	{
		public static string m_strInputField
		{
			get { return Hy.API.strGetVariable("Project.strInputField"); }
		}
		public static float m_fVecX1
		{
			get { return (float)Hy.API.dGetVariable("Project.fVecX1"); }
		}
		public static float m_fVecY1
		{
			get { return (float)Hy.API.dGetVariable("Project.fVecY1"); }
		}
		public static float m_fVecZ1
		{
			get { return (float)Hy.API.dGetVariable("Project.fVecZ1"); }
		}
		public static float m_fVecX2
		{
			get { return (float)Hy.API.dGetVariable("Project.fVecX2"); }
		}
		public static float m_fVecY2
		{
			get { return (float)Hy.API.dGetVariable("Project.fVecY2"); }
		}
		public static float m_fVecZ2
		{
			get { return (float)Hy.API.dGetVariable("Project.fVecZ2"); }
		}
		public static float m_fSlider1
		{
			get { return (float)Hy.API.dGetVariable("Project.fSlider1"); }
		}
		public static float m_fSlider2
		{
			get { return (float)Hy.API.dGetVariable("Project.fSlider2"); }
		}
		public static void ReadVariables()
		{
			System.Diagnostics.Debug.Print($@"m_fSlider1 : {m_fSlider1}");
			System.Diagnostics.Debug.Print($@"m_fSlider2 : {m_fSlider2}");
			System.Diagnostics.Debug.Print($@"m_strInputField : {m_strInputField}");
			System.Diagnostics.Debug.Print($@"m_fVecInputFieldX1 : {m_fVecX1}");
			System.Diagnostics.Debug.Print($@"m_fVecInputFieldY1 : {m_fVecY1}");
			System.Diagnostics.Debug.Print($@"m_fVecInputFieldZ1 : {m_fVecZ1}");
			System.Diagnostics.Debug.Print($@"m_fVecInputFieldX2 : {m_fVecX2}");
			System.Diagnostics.Debug.Print($@"m_fVecInputFieldY2 : {m_fVecY2}");
			System.Diagnostics.Debug.Print($@"m_fVecInputFieldZ2 : {m_fVecZ2}");
		}
	}
}
