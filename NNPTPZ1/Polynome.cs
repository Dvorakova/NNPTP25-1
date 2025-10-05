using System.Collections.Generic;

namespace NNPTPZ1.Mathematics
{
    public class Polynome
    {
        public List<ComplexNumber> Coefficients { get; private set; }

        public Polynome() => Coefficients = new List<ComplexNumber>();

        public void Add(ComplexNumber coefficient) =>
            Coefficients.Add(coefficient);

        /// <summary>
        /// Returns a new polynomial representing the derivative of this instance.
        /// </summary>
        /// <returns>The derived polynomial.</returns>

        public Polynome Derive()
        {
            Polynome result = new Polynome();
            for (int i = 1; i < Coefficients.Count; i++)
            {
                result.Coefficients.Add(Coefficients[i].Multiply(new ComplexNumber { RealPart = i }));
            }

            return result;
        }

        /// <summary>
        /// Evaluates the polynomial at a specified real value.
        /// </summary>
        /// <param name="point">The real value at which to evaluate the polynomial.</param>
        /// <returns>The resulting complex value.</returns>

        public ComplexNumber Eval(double point) =>
            Eval(new ComplexNumber { RealPart = point });

        /// <summary>
        /// Evaluates the polynomial at a specified complex value.
        /// </summary>
        /// <param name="input">The complex value at which to evaluate the polynomial.</param>
        /// <returns>The resulting complex value.</returns>
        public ComplexNumber Eval(ComplexNumber input)
        {
            ComplexNumber result = ComplexNumber.Zero;

            for (int i = 0; i < Coefficients.Count; i++)
            {
                ComplexNumber coefficient = Coefficients[i];
                ComplexNumber powerValue = input;
                int power = i;

                if (i > 0)
                {
                    for (int j = 0; j < power - 1; j++)
                        powerValue = powerValue.Multiply(input);

                    coefficient = coefficient.Multiply(powerValue);
                }

                result = result.Add(coefficient);
            }

            return result;
        }

        public override string ToString()
        {
            string result = "";
            
            for (int i = 0; i < Coefficients.Count; i++)
            {
                result += Coefficients[i];
                if (i > 0)
                {                    
                    for (int j = 0; j < i; j++)
                    {
                        result += "x";
                    }
                }
                if (i + 1 < Coefficients.Count)
                    result += " + ";
            }
            return result;
        }
    }
}