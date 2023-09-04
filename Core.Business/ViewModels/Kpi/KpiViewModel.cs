using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Kpi
{
    public class KpiViewModel
    {
        public KpiViewModel()
        {
            
        }
        public string FullName { get; set; }
        public int P { get; set; }
        public int A { get; set; }
        public int L { get; set; }
        public int M { get; set; }
        public int S { get; set; }
        public int RGI { get; set; }
        public int RGO { get; set; }
        public int RRI { get; set; }
        public int RRO { get; set; }
        public int V { get; set; }
        public int F2F { get; set; }
        public long TYFCB { get; set; }
        public int CEU { get; set; }
        public double R { get; set; }
        public double AvgF2F { get; set; }
        public double AvgV { get; set; }
        public int RG { get; set; }
        public int RR { get; set; }

        public int PointA { get; set; }
        public string ColorA { get; set; }
        public int PointL { get; set; }
        public string ColorL { get; set; }
        public int PointRG { get; set; }
        public string ColorRG { get; set; }
        public int PointF2F { get; set; }
        public string ColorF2F { get; set; }
        public int PointV { get; set; }
        public string ColorV { get; set; }
        public int PointT { get; set; }
        public string ColorT { get; set; }
        public int PointTYFCB { get; set; }
        public string ColorTYFCB { get; set; }
        public int Pts { get; set; }
        public string ColorRow { get; set; }
    }

}
