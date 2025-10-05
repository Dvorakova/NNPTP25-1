using System;
using System.Collections.Generic;
using System.Drawing;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    /// <summary>
/// Generates a Newton fractal image based on polynomial root iterations.
/// </summary>
/// <remarks>
/// See: https://en.wikipedia.org/wiki/Newton_fractal
/// </remarks>

    class Program
    {
        static int[] imageDimensions;
        static string outputFilePath;
        static Bitmap bitmap;
        static double xMin, yMin, xStep, yStep;
        static List<ComplexNumber> roots;
        static Polynome polynomial, polynomialDerivative;
        static Color[] rootColors;
        static int maxRootIndex;
        private const int MaxIterations = 30;
        private const double NewtonIterationTolerance = 0.5;
        private const double RootProximityThreshold = 0.01;
        private const string DefaultOutputFilePath = "../../../out.png";
        private const double MinimalCoordinateValue = 0.0001;

        static void Main(string[] args)
        {            
            ParseCommandLineArguments(args);
            
            PrepareCalculationEnvironment();
            maxRootIndex = ProcessNewtonIterationAlgorithm();
            SaveOutputImage();
        }

        private static void SaveOutputImage()
        {
            bitmap.Save(outputFilePath ?? DefaultOutputFilePath);
        }

        private static int ProcessNewtonIterationAlgorithm()
        {
            for (int i = 0; i < imageDimensions[0]; i++)
            {
                for (int j = 0; j < imageDimensions[1]; j++)
                {
                    ProcessPixelUsingNewtonIteration(i, j);
                }
            }

            return maxRootIndex;
        }

        private static void ProcessPixelUsingNewtonIteration(int i, int j)
        {
            ComplexNumber point = PrepareComplexNumberAtPoint(i, j);

            int convergenceIterations = ProcessNewtonIterationOnSinglePixel(ref point);
            int rootIndex = FindPolynomeRoot(point);
            ColorizePixel(i, j, convergenceIterations, rootIndex);
        }

        /// <summary>
        /// Colors a single pixel based on the root it converged to and the number of iterations.
        /// </summary>
        /// <param name="i">Row index of the pixel.</param>
        /// <param name="j">Column index of the pixel.</param>
        /// <param name="convergenceIterations">Number of iterations performed for this pixel.</param>
        /// <param name="rootIndex">Index of the converged root.</param>
        private static void ColorizePixel(int i, int j, int convergenceIterations, int rootIndex)
        {
            Color color = rootColors[rootIndex % rootColors.Length];
            color = Color.FromArgb(
                Math.Min(Math.Max(0, color.R - convergenceIterations * 2), 255), 
                Math.Min(Math.Max(0, color.G - convergenceIterations * 2), 255), 
                Math.Min(Math.Max(0, color.B - convergenceIterations * 2), 255)
                );
            bitmap.SetPixel(j, i, color);
        }

        /// <summary>
        /// Finds the index of the root that is closest to the given complex point.
        /// Adds the point as a new root if convergenceIterations is not yet known.
        /// </summary>
        /// <param name="point">The complex point to check against known roots.</param>
        /// <returns>The index of the corresponding root.</returns>
        private static int FindPolynomeRoot(ComplexNumber point)
        {
            bool knownRoot = false;
            int rootIndex = 0;
            for (int i = 0; i < roots.Count; i++)
            {
                if (point.Subtract(roots[i]).GetAbsoluteValue() <= RootProximityThreshold)
                {
                    knownRoot = true;
                    rootIndex = i;
                }
            }
            if (!knownRoot)
            {
                roots.Add(point);
                rootIndex = roots.Count;
                maxRootIndex = rootIndex + 1;
            }

            return rootIndex;
        }

        /// <summary>
        /// Applies Newton's iteration to refine the complex point until convergence.
        /// </summary>
        /// <param name="point">The complex number representing the pixel position (modified by reference).</param>
        /// <returns>The number of iterations performed.</returns>
        private static int ProcessNewtonIterationOnSinglePixel(ref ComplexNumber point)
        {
            int iteration = 0;
            for (int i = 0; i < MaxIterations; i++)
            {
                ComplexNumber differential = polynomial.Eval(point).Divide(polynomialDerivative.Eval(point));
                point = point.Subtract(differential);

                if (differential.GetAbsoluteValue() >= NewtonIterationTolerance)
                {
                    i--;
                }
                iteration++;
            }

            return iteration;
        }

        /// <summary>
        /// Prepares a complex number representing the coordinates of a pixel
        /// in the fractal's "world" coordinate system.
        /// </summary>
        /// <param name="i">Row index of the pixel.</param>
        /// <param name="j">Column index of the pixel.</param>
        /// <returns>A ComplexNumber representing the pixel's position.</returns>
        private static ComplexNumber PrepareComplexNumberAtPoint(int i, int j)
        {
            double y = yMin + i * yStep;
            double x = xMin + j * xStep;

            ComplexNumber point = new ComplexNumber()
            {
                RealPart = x,
                ImaginaryPart = y
            };

            if (point.RealPart == 0)
                point.RealPart = MinimalCoordinateValue;
            if (point.ImaginaryPart == 0)
                point.ImaginaryPart = MinimalCoordinateValue;

            return point;
        }

        private static void PrepareCalculationEnvironment()
        {
            roots = new List<ComplexNumber>();
            polynomial = new Polynome();
            polynomial.Coefficients.Add(new ComplexNumber() { RealPart = 1 });
            polynomial.Coefficients.Add(ComplexNumber.Zero);
            polynomial.Coefficients.Add(ComplexNumber.Zero);
            polynomial.Coefficients.Add(new ComplexNumber() { RealPart = 1 });
            polynomialDerivative = polynomial.Derive();
            Console.WriteLine(polynomial);
            Console.WriteLine(polynomialDerivative);

            rootColors = new Color[]
            {
                Color.Red, 
                Color.Blue,
                Color.Green,
                Color.Yellow,
                Color.Orange,
                Color.Fuchsia,
                Color.Gold,
                Color.Cyan,
                Color.Magenta
            };
            maxRootIndex = 0;
        }

        private static void ParseCommandLineArguments(string[] args)
        {
            const int WidthIndex = 0;
            const int HeightIndex = 1;
            const int XMinIndex = 2;
            const int XMaxIndex = 3;
            const int YMinIndex = 4;
            const int YMaxIndex = 5;
            const int OutputPathIndex = 6;

            imageDimensions = new int[2];
            imageDimensions[0] = int.Parse(args[WidthIndex]);
            imageDimensions[1] = int.Parse(args[HeightIndex]);

            double xMinValue = double.Parse(args[XMinIndex]);
            double xMaxValue = double.Parse(args[XMaxIndex]);
            double yMinValue = double.Parse(args[YMinIndex]);
            double yMaxValue = double.Parse(args[YMaxIndex]);

            outputFilePath = args[OutputPathIndex];
            bitmap = new Bitmap(imageDimensions[WidthIndex], imageDimensions[HeightIndex]);

            xMin = xMinValue;
            double xMax = xMaxValue;
            yMin = yMinValue;
            double yMax = yMaxValue;

            xStep = (xMax - xMin) / imageDimensions[WidthIndex];
            yStep = (yMax - yMin) / imageDimensions[HeightIndex];
        }
    }
}
