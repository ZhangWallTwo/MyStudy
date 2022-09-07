using GeneticSharp.Domain.Chromosomes;
using System;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Commons;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;
using System.Diagnostics;

namespace GeneticSharp.Runner.UnityApp.Commons
{
    public abstract class BitStringChromosome<TPhenotypeEntity> : BinaryChromosomeBase
        where TPhenotypeEntity : IPhenotypeEntity
    {
        private TPhenotypeEntity[] m_phenotypeEntities;
        private string m_originalValueStringRepresentation;

        protected BitStringChromosome()
            : base(2)
        {
        }

        protected void SetPhenotypes(params TPhenotypeEntity[] phenotypeEntities)
        {
            if (phenotypeEntities.Length == 0)
            {
                throw new ArgumentException("At least one phenotype entity should be informed.", nameof(phenotypeEntities));
            }

            m_phenotypeEntities = phenotypeEntities;
            Resize(m_phenotypeEntities.Sum(e => e.Phenotypes.Sum(p => p.Length)));
        }

        public virtual TPhenotypeEntity[] GetPhenotypes()
        {
            var genes = GetGenes();
            var skip = 0;
            var entityLength = 0;

            foreach (var entity in m_phenotypeEntities)
            {
                entityLength = entity.GetTotalBits();
                entity.Load(genes.Skip(skip).Take(entityLength).Select(g => (int)g.Value));
                skip += entityLength;
            }

            return m_phenotypeEntities;
        }

        protected override void CreateGenes()//유전자 생성
        {
            var valuesLength = m_phenotypeEntities.Sum(p => p.Phenotypes.Length);
            var originalValues = new double[valuesLength];
            var totalBits = new int[valuesLength];
            var fractionBits = new int[valuesLength];
            IPhenotype phenotype;

            int valueIndex = 0;
            foreach (var entity in m_phenotypeEntities)
            {
                for (int i = 0; i < entity.Phenotypes.Length; i++)
                {
                    phenotype = entity.Phenotypes[i];
                    originalValues[valueIndex] = phenotype.RandomValue();
                    totalBits[valueIndex] = phenotype.Length;
                    fractionBits[valueIndex] = 0;

                    valueIndex++;
                }
            }
            
            m_originalValueStringRepresentation = String.Join(
                String.Empty,
                BinaryStringRepresentation.ToRepresentation(
                    originalValues,
                    totalBits,
                    fractionBits));

            base.CreateGenes();
        }

        public override Gene GenerateGene(int geneIndex)//유전자 생성
        {
            //Int32 test = Convert.ToInt32(m_originalValueStringRepresentation[geneIndex].ToString());//test
            //int a = 1;//test

            //Test
            // 01001001/00010011
            //    15     13
            //string test = "00000011";
            //int rst = Convert.ToInt32(test, 2);

            //int result = Convert.ToInt32(m_originalValueStringRepresentation[geneIndex].ToString(),2);
            //Console.WriteLine(result);
            string tes = m_originalValueStringRepresentation;
            int size = tes.Length;
            List<char> rst = ConvertStringToInt();
            int test = 1;
            return new Gene(Convert.ToInt32(m_originalValueStringRepresentation[geneIndex].ToString()));//2진수 나옴!!
        }

        public List<char> ConvertStringToInt()
        {
            //string test = "abcdefg";
            //test = test.Remove(0, 3);
            string str = m_originalValueStringRepresentation;
            string binaryNumbers = "";
            string decimalNumbers = "";
            while (str.Length!=0)
            {
                for(int i=0; i < 8; i++)
                {
                    binaryNumbers += str[i];

                }
                str = str.Remove(0, 8);
                int tmp = Convert.ToInt32(binaryNumbers, 2);
                decimalNumbers += tmp.ToString();
                binaryNumbers = "";
            }
            List<char> result = new List<char> { };
            //for(int i = 0; i < result2.Length; i++)
            //{
            //    final.Add(result2[i]);
            //}

           
            return result;
        }

    }

    public interface IPhenotype
    {
        string Name { get; }
        int Length { get; }
        double MinValue { get; }
        double MaxValue { get; }
        double Value { get; set; }

        double RandomValue();
    }

    public interface IPhenotypeEntity
    {
        IPhenotype[] Phenotypes { get; }
        void Load(IEnumerable<int> entityGenes);
    }

    public static class PhenotypeEntityExtensions
    {
        public static int GetTotalBits(this IPhenotypeEntity entity)
        {
            return entity.Phenotypes.Sum(p => p.Length);
        }
    }

    public abstract class PhenotypeEntityBase : IPhenotypeEntity
    {
        public IPhenotype[] Phenotypes { get; protected set; }

        public void Load(IEnumerable<int> entityGenes)
        {
            var skip = 0;
           
            foreach(var p in Phenotypes)
            {
                p.Value = GetValue(entityGenes, skip, p);
                skip += p.Length;
            }
        }
      
        private double GetValue(IEnumerable<int> genes, int skip, IPhenotype phenotype)
        {
            var representation = string.Join(String.Empty, genes.Skip(skip).Take(phenotype.Length));
            var value = (float)BinaryStringRepresentation.ToDouble(representation, 0);

            if (value < phenotype.MinValue)
                return phenotype.MinValue;

            if (value > phenotype.MaxValue)
                return phenotype.MaxValue;

            return value;
        }
    }

    [DebuggerDisplay("{Name} = {MinValue} <= {Value} <= {MaxValue}")]
    public class Phenotype : IPhenotype
    {
        public Phenotype(string name, int length)
        {
            Name = name;
            Length = length;
        }

        public string Name { get; }
        public int Length { get; }
        public double MinValue { get; set; } = 0;
        public double MaxValue { get; set; } = 100;
        public virtual double Value { get; set; }
    
        public virtual double RandomValue()
        {
            return RandomizationProvider.Current.GetDouble(MinValue, MaxValue + 1);
        }
    }


}