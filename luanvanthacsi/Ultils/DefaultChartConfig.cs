using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hrm.Web.Share.Ultils
{
    public class DefaultConfig
    {
    }

    public class DefaultTableConfig
    {
        public static string ScrollBarWidth = "10px";
    }

    public class DefaultInputConfig
    {
        public static int DebounceMilliseconds = 1000;
    }

    public class DefaultChartConfig
    {
        public static string JsDonutChartConfig = @"
        {
            meta: 
            {
              value: 
              {
                formatter: (v) => `${v}`,
              },
            },
            label: 
            {
              type: 'inner',
              offset: '-50%',
              style: {textAlign: 'center',},
              autoRotate: false,
              content: '',
            },
            statistic: 
            {
              title: 
                {
                    offsetY: -4,
                    customHtml: (container, view, datum) => 
                    {
                      const { width, height } = container.getBoundingClientRect();
                      const d = Math.sqrt(Math.pow(width / 2, 2) + Math.pow(height / 2, 2));
                      const text = datum ? datum.type : 'Tổng';
                      return `<div>${text}</div>`;        
                    },
                },
              content: 
                {
                    offsetY: 4,
                    style: {fontSize: '32px',},
                    customHtml: (container, view, datum, data) => 
                    {
                      const { width } = container.getBoundingClientRect();
                      const text = datum ? ` ${datum.value}` : `${data.reduce((r, d) => r + d.value, 0)}`;
                      return `<div >${text}</div>`;
                    },
                },
            },
        }";
    }

}
