using System;

namespace NNPTPZ1.Mathematics
{
    public class ComplexNumber
    {
        public double RealPart { get; set; }
        public double ImaginaryPart { get; set; }

        public static readonly ComplexNumber Zero = new ComplexNumber();

        public ComplexNumber Add(ComplexNumber other) =>
            new ComplexNumber
            {
                RealPart = RealPart + other.RealPart,
                ImaginaryPart = ImaginaryPart + other.ImaginaryPart
            };

        public ComplexNumber Subtract(ComplexNumber other) =>
            new ComplexNumber
            {
                RealPart = RealPart - other.RealPart,
                ImaginaryPart = ImaginaryPart - other.ImaginaryPart
            };

        public ComplexNumber Multiply(ComplexNumber other) =>
            new ComplexNumber
            {
                RealPart = RealPart * other.RealPart - ImaginaryPart * other.ImaginaryPart,
                ImaginaryPart = RealPart * other.ImaginaryPart + ImaginaryPart * other.RealPart
            };

        public ComplexNumber Divide(ComplexNumber other)
        {
            ComplexNumber numerator = Multiply(new ComplexNumber
            {
                RealPart = other.RealPart,
                ImaginaryPart = -other.ImaginaryPart
            });

            double denominator = other.RealPart * other.RealPart + other.ImaginaryPart * other.ImaginaryPart;

            return new ComplexNumber
            {
                RealPart = numerator.RealPart / denominator,
                ImaginaryPart = numerator.ImaginaryPart / denominator
            };
        }

        public double GetAbsoluteValue() =>
            Math.Sqrt(RealPart * RealPart + ImaginaryPart * ImaginaryPart);

        public double GetAngleInRadians() =>
            Math.Atan(ImaginaryPart / RealPart);

        public override bool Equals(object obj) =>
            obj is ComplexNumber number &&
            RealPart == number.RealPart &&
            ImaginaryPart == number.ImaginaryPart;

        public override string ToString() =>
            $"({RealPart} + {ImaginaryPart}i)";
    }
}
