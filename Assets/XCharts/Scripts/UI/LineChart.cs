﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XCharts
{
    [AddComponentMenu("XCharts/LineChart", 13)]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class LineChart : CoordinateChart
    {
        [SerializeField] private Line m_Line = Line.defaultLine;

        public Line line { get { return m_Line; } }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            m_Line = Line.defaultLine;
            m_Title.text = "LineChart";
            RemoveData();
            AddSerie("serie1", SerieType.Line);
            for (int i = 0; i < 5; i++)
            {
                AddXAxisData("x" + (i + 1));
                AddData(0, Random.Range(10, 90));
            }
        }
#endif

        protected override void DrawChart(VertexHelper vh)
        {
            base.DrawChart(vh);
            if (m_YAxises[0].type == Axis.AxisType.Category
                || m_YAxises[1].type == Axis.AxisType.Category)
            {
                DrawYCategory(vh);
            }
            else
            {
                DrawXCategory(vh);
            }
        }


        private void DrawXCategory(VertexHelper vh)
        {
            var stackSeries = m_Series.GetStackSeries();
            int seriesCount = stackSeries.Count;

            int serieCount = 0;
            List<Vector3> points = new List<Vector3>();
            List<Color> colorList = new List<Color>();
            int dataCount = 0;
            for (int j = 0; j < seriesCount; j++)
            {
                var seriesCurrHig = new Dictionary<int, float>();
                var serieList = stackSeries[j];

                for (int n = 0; n < serieList.Count; n++)
                {
                    Serie serie = serieList[n];
                    DrawXLineSerie(vh, serieCount, serie, ref dataCount, ref points, ref colorList, ref seriesCurrHig);
                    if (serie.show)
                    {
                        serieCount++;
                    }
                }
                // draw point
                if (m_Line.point)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        Vector3 p = points[i];
                        float pointWid = m_Line.pointWidth;
                        if (m_Tooltip.show && i % dataCount == m_Tooltip.dataIndex - 1)
                        {
                            pointWid = pointWid * 1.8f;
                        }
                        if (m_Theme == Theme.Dark)
                        {

                            ChartHelper.DrawCricle(vh, p, pointWid, colorList[i],
                                (int)m_Line.pointWidth * 5);
                        }
                        else
                        {
                            ChartHelper.DrawCricle(vh, p, pointWid, Color.white);
                            ChartHelper.DrawDoughnut(vh, p, pointWid - m_Line.tickness,
                                pointWid, 0, 360, colorList[i]);
                        }
                    }
                }
            }

            //draw tooltip line
            if (m_Tooltip.show && m_Tooltip.dataIndex > 0)
            {
                for (int i = 0; i < m_XAxises.Count; i++)
                {
                    var axis = m_XAxises[i];
                    if (!axis.show) continue;
                    float splitWidth = axis.GetSplitWidth(coordinateWid, m_DataZoom);
                    float px = coordinateX + (m_Tooltip.xValues[i] - 1) * splitWidth
                        + (axis.boundaryGap ? splitWidth / 2 : 0);
                    Vector2 sp = new Vector2(px, coordinateY);
                    Vector2 ep = new Vector2(px, coordinateY + coordinateHig);
                    ChartHelper.DrawLine(vh, sp, ep, m_Coordinate.tickness, m_ThemeInfo.tooltipLineColor);
                    if (m_Tooltip.crossLabel)
                    {
                        sp = new Vector2(coordinateX, m_Tooltip.pointerPos.y);
                        ep = new Vector2(coordinateX + coordinateWid, m_Tooltip.pointerPos.y);
                        DrawSplitLine(vh, true, Axis.SplitLineType.Dashed, sp, ep, m_ThemeInfo.tooltipLineColor);
                    }
                }
            }
        }

        private void DrawYCategory(VertexHelper vh)
        {
            var stackSeries = m_Series.GetStackSeries();
            int seriesCount = stackSeries.Count;
            int serieCount = 0;
            List<Vector3> points = new List<Vector3>();
            List<Color> colors = new List<Color>();
            int dataCount = 0;
            for (int j = 0; j < seriesCount; j++)
            {
                var seriesHig = new Dictionary<int, float>();
                var serieList = stackSeries[j];

                for (int n = 0; n < serieList.Count; n++)
                {
                    Serie serie = serieList[n];
                    DrawYLineSerie(vh, serieCount, serie, ref dataCount, ref points, ref colors, ref seriesHig);
                    if (serie.show)
                    {
                        serieCount++;
                    }
                }
                // draw point
                if (m_Line.point)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        Vector3 p = points[i];
                        float pointWid = m_Line.pointWidth;
                        if (m_Tooltip.show && i % dataCount == m_Tooltip.dataIndex - 1)
                        {
                            pointWid = pointWid * 1.8f;
                        }
                        if (m_Theme == Theme.Dark)
                        {
                            ChartHelper.DrawCricle(vh, p, pointWid, colors[i], (int)m_Line.pointWidth * 5);
                        }
                        else
                        {
                            ChartHelper.DrawCricle(vh, p, pointWid, Color.white);
                            ChartHelper.DrawDoughnut(vh, p, pointWid - m_Line.tickness, pointWid, 0, 360, colors[i]);
                        }
                    }
                }
            }
            //draw tooltip line
            if (m_Tooltip.show && m_Tooltip.dataIndex > 0)
            {
                for (int i = 0; i < m_YAxises.Count; i++)
                {
                    var axis = m_YAxises[i];
                    if (!axis.show) continue;
                    float splitWidth = axis.GetSplitWidth(coordinateHig, m_DataZoom);
                    float pY = coordinateY + (m_Tooltip.yValues[i] - 1) * splitWidth + (axis.boundaryGap ? splitWidth / 2 : 0);
                    Vector2 sp = new Vector2(coordinateX, pY);
                    Vector2 ep = new Vector2(coordinateX + coordinateWid, pY);
                    ChartHelper.DrawLine(vh, sp, ep, m_Coordinate.tickness, m_ThemeInfo.tooltipFlagAreaColor);
                    if (m_Tooltip.crossLabel)
                    {
                        sp = new Vector2(m_Tooltip.pointerPos.x, coordinateY);
                        ep = new Vector2(m_Tooltip.pointerPos.x, coordinateY + coordinateHig);
                        DrawSplitLine(vh, false, Axis.SplitLineType.Dashed, sp, ep, m_ThemeInfo.tooltipLineColor);
                    }
                }
            }
        }

        private void DrawXLineSerie(VertexHelper vh, int serieIndex, Serie serie, ref int dataCount,
            ref List<Vector3> points, ref List<Color> colors, ref Dictionary<int, float> seriesHig)
        {
            if (!IsActive(serie.name)) return;
            List<Vector3> lastPoints = new List<Vector3>();
            List<Vector3> lastSmoothPoints = new List<Vector3>();
            List<Vector3> smoothPoints = new List<Vector3>();
            List<float> yData = serie.GetYDataList(m_DataZoom);
            List<float> xData = serie.GetXDataList(m_DataZoom);

            Color color = m_ThemeInfo.GetColor(serieIndex);
            Vector3 lp = Vector3.zero;
            Vector3 np = Vector3.zero;
            var yAxis = m_YAxises[serie.axisIndex];
            var xAxis = m_XAxises[serie.axisIndex];
            if (!xAxis.show) xAxis = m_XAxises[(serie.axisIndex + 1) % m_XAxises.Count];
            float scaleWid = xAxis.GetDataWidth(coordinateWid, m_DataZoom);
            float startX = coordinateX + (xAxis.boundaryGap ? scaleWid / 2 : 0);
            int maxCount = maxShowDataNumber > 0 ?
                (maxShowDataNumber > yData.Count ? yData.Count : maxShowDataNumber)
                : yData.Count;
            dataCount = (maxCount - minShowDataNumber);
            if (m_Line.area && points.Count > 0)
            {
                if (!m_Line.smooth && points.Count > 0)
                {
                    for (int m = points.Count - dataCount; m < points.Count; m++)
                    {
                        lastPoints.Add(points[m]);
                    }
                }
                else if (m_Line.smooth && smoothPoints.Count > 0)
                {
                    for (int m = 0; m < smoothPoints.Count; m++)
                    {
                        lastSmoothPoints.Add(smoothPoints[m]);
                    }
                    smoothPoints.Clear();
                }
            }
            int smoothPointCount = 1;
            for (int i = minShowDataNumber; i < maxCount; i++)
            {
                if (!seriesHig.ContainsKey(i))
                {
                    seriesHig[i] = 0;
                }
                float yValue = yData[i];
                float yDataHig;
                if (xAxis.IsValue())
                {
                    float xValue = i > xData.Count - 1 ? 0 : xData[i];
                    float pX = coordinateX + m_Coordinate.tickness;
                    float pY = seriesHig[i] + coordinateY + m_Coordinate.tickness;
                    float xDataHig = (xValue - xAxis.minValue) / (xAxis.maxValue - xAxis.minValue) * coordinateWid;
                    yDataHig = (yValue - yAxis.minValue) / (yAxis.maxValue - yAxis.minValue) * coordinateHig;
                    np = new Vector3(pX + xDataHig, pY + yDataHig);
                }
                else
                {
                    float pX = startX + i * scaleWid;
                    float pY = seriesHig[i] + coordinateY + m_Coordinate.tickness;
                    yDataHig = (yValue - yAxis.minValue) / (yAxis.maxValue - yAxis.minValue) * coordinateHig;
                    np = new Vector3(pX, pY + yDataHig);
                }

                if (i > 0)
                {
                    if (m_Line.step)
                    {
                        Vector2 middle1, middle2;
                        switch (m_Line.stepTpe)
                        {
                            case Line.StepType.Start:
                                middle1 = new Vector2(lp.x, np.y + m_Line.tickness);
                                middle2 = new Vector2(lp.x - m_Line.tickness, np.y);
                                ChartHelper.DrawLine(vh, lp, middle1, m_Line.tickness, color);
                                ChartHelper.DrawLine(vh, middle2, np, m_Line.tickness, color);
                                if (m_Line.area)
                                {
                                    Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                                    ChartHelper.DrawPolygon(vh, new Vector2(middle1.x, coordinateY), middle1, np,
                                        new Vector2(np.x, coordinateY), areaColor);
                                }
                                break;
                            case Line.StepType.Middle:
                                middle1 = new Vector2((lp.x + np.x) / 2 + m_Line.tickness, lp.y);
                                middle2 = new Vector2((lp.x + np.x) / 2 - m_Line.tickness, np.y);
                                ChartHelper.DrawLine(vh, lp, middle1, m_Line.tickness, color);
                                ChartHelper.DrawLine(vh, new Vector2(middle1.x - m_Line.tickness, middle1.y),
                                    new Vector2(middle2.x + m_Line.tickness, middle2.y), m_Line.tickness, color);
                                ChartHelper.DrawLine(vh, middle2, np, m_Line.tickness, color);
                                if (m_Line.area)
                                {
                                    Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                                    ChartHelper.DrawPolygon(vh, new Vector2(lp.x, coordinateY), lp, middle1,
                                        new Vector2(middle1.x, coordinateY), areaColor);
                                    ChartHelper.DrawPolygon(vh, new Vector2(middle2.x + 2 * m_Line.tickness, coordinateY),
                                        new Vector2(middle2.x + 2 * m_Line.tickness, middle2.y), np,
                                        new Vector2(np.x, coordinateY), areaColor);
                                }
                                break;
                            case Line.StepType.End:
                                middle1 = new Vector2(np.x + m_Line.tickness, lp.y);
                                middle2 = new Vector2(np.x, lp.y);
                                ChartHelper.DrawLine(vh, lp, middle1, m_Line.tickness, color);
                                ChartHelper.DrawLine(vh, middle2, np, m_Line.tickness, color);
                                if (m_Line.area)
                                {
                                    Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                                    ChartHelper.DrawPolygon(vh, new Vector2(lp.x, coordinateY), lp,
                                        new Vector2(middle1.x - m_Line.tickness, middle1.y),
                                        new Vector2(middle1.x - m_Line.tickness, coordinateY), areaColor);
                                }
                                break;
                        }
                    }
                    else if (m_Line.smooth)
                    {
                        Vector3[] list;
                        if (xAxis.IsValue()) list = ChartHelper.GetBezierListVertical(lp, np, m_Line.smoothStyle);
                        else list = ChartHelper.GetBezierList(lp, np, m_Line.smoothStyle);
                        Vector3 start, to;
                        start = list[0];
                        for (int k = 1; k < list.Length; k++)
                        {
                            smoothPoints.Add(list[k]);
                            to = list[k];
                            ChartHelper.DrawLine(vh, start, to, m_Line.tickness, color);

                            if (m_Line.area)
                            {
                                Vector3 alp = new Vector3(start.x, start.y - m_Line.tickness);
                                Vector3 anp = new Vector3(to.x, to.y - m_Line.tickness);
                                Vector3 tnp = serieIndex > 0 ?
                                    (smoothPointCount > lastSmoothPoints.Count - 1 ?
                                    new Vector3(lastSmoothPoints[lastSmoothPoints.Count - 1].x,
                                        lastSmoothPoints[lastSmoothPoints.Count - 1].y + m_Line.tickness) :
                                    new Vector3(lastSmoothPoints[smoothPointCount].x,
                                        lastSmoothPoints[smoothPointCount].y + m_Line.tickness)) :
                                    new Vector3(to.x, coordinateY + m_Coordinate.tickness);
                                Vector3 tlp = serieIndex > 0 ?
                                    (smoothPointCount > lastSmoothPoints.Count - 1 ?
                                    new Vector3(lastSmoothPoints[lastSmoothPoints.Count - 2].x,
                                        lastSmoothPoints[lastSmoothPoints.Count - 2].y + m_Line.tickness) :
                                    new Vector3(lastSmoothPoints[smoothPointCount - 1].x,
                                        lastSmoothPoints[smoothPointCount - 1].y + m_Line.tickness)) :
                                    new Vector3(start.x, coordinateY + m_Coordinate.tickness);
                                Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                                ChartHelper.DrawPolygon(vh, alp, anp, tnp, tlp, areaColor);
                            }
                            smoothPointCount++;
                            start = to;
                        }
                    }
                    else
                    {
                        ChartHelper.DrawLine(vh, lp, np, m_Line.tickness, color);
                        if (m_Line.area)
                        {
                            Vector3 alp = new Vector3(lp.x, lp.y - m_Line.tickness);
                            Vector3 anp = new Vector3(np.x, np.y - m_Line.tickness);
                            Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                            var cross = ChartHelper.GetIntersection(lp, np, new Vector3(coordinateX, coordinateY),
                                new Vector3(coordinateX + coordinateWid, coordinateY));
                            if (cross == Vector3.zero)
                            {
                                Vector3 tnp = serieIndex > 0 ?
                                    new Vector3(lastPoints[i].x, lastPoints[i].y + m_Line.tickness) :
                                    new Vector3(np.x, coordinateY + m_Coordinate.tickness);
                                Vector3 tlp = serieIndex > 0 ?
                                    new Vector3(lastPoints[i - 1].x, lastPoints[i - 1].y + m_Line.tickness) :
                                    new Vector3(lp.x, coordinateY + m_Coordinate.tickness);
                                ChartHelper.DrawPolygon(vh, alp, anp, tnp, tlp, areaColor);
                            }
                            else
                            {
                                Vector3 cross1 = new Vector3(cross.x, cross.y + (alp.y > coordinateY ? m_Coordinate.tickness : -m_Coordinate.tickness));
                                Vector3 cross2 = new Vector3(cross.x, cross.y + (anp.y > coordinateY ? m_Coordinate.tickness : -m_Coordinate.tickness));
                                Vector3 xp1 = new Vector3(alp.x, coordinateY + (alp.y > coordinateY ? m_Coordinate.tickness : -m_Coordinate.tickness));
                                Vector3 xp2 = new Vector3(anp.x, coordinateY + (anp.y > coordinateY ? m_Coordinate.tickness : -m_Coordinate.tickness));
                                ChartHelper.DrawTriangle(vh, alp, cross1, xp1, areaColor);
                                ChartHelper.DrawTriangle(vh, anp, cross2, xp2, areaColor);
                            }
                        }
                    }
                }
                if (m_Line.point)
                {
                    points.Add(np);
                    colors.Add(color);
                }
                seriesHig[i] += yDataHig;
                lp = np;
            }
        }

        private void DrawYLineSerie(VertexHelper vh, int serieIndex, Serie serie, ref int dataCount,
            ref List<Vector3> points, ref List<Color> colors, ref Dictionary<int, float> seriesHig)
        {
            if (!IsActive(serie.name)) return;
            List<Vector3> lastPoints = new List<Vector3>();
            List<Vector3> lastSmoothPoints = new List<Vector3>();
            List<Vector3> smoothPoints = new List<Vector3>();

            Color color = m_ThemeInfo.GetColor(serieIndex);
            Vector3 lp = Vector3.zero;
            Vector3 np = Vector3.zero;
            var xAxis = m_XAxises[serie.axisIndex];
            var yAxis = m_YAxises[serie.axisIndex];
            if (!yAxis.show) yAxis = m_YAxises[(serie.axisIndex + 1) % m_YAxises.Count];
            float scaleWid = yAxis.GetDataWidth(coordinateHig, m_DataZoom);
            float startY = coordinateY + (yAxis.boundaryGap ? scaleWid / 2 : 0);
            int maxCount = maxShowDataNumber > 0 ?
                (maxShowDataNumber > serie.yData.Count ? serie.yData.Count : maxShowDataNumber)
                : serie.yData.Count;
            dataCount = (maxCount - minShowDataNumber);
            if (m_Line.area && points.Count > 0)
            {
                if (!m_Line.smooth && points.Count > 0)
                {
                    for (int m = points.Count - dataCount; m < points.Count; m++)
                    {
                        lastPoints.Add(points[m]);
                    }
                }
                else if (m_Line.smooth && smoothPoints.Count > 0)
                {
                    for (int m = 0; m < smoothPoints.Count; m++)
                    {
                        lastSmoothPoints.Add(smoothPoints[m]);
                    }
                    smoothPoints.Clear();
                }
            }
            int smoothPointCount = 1;
            for (int i = minShowDataNumber; i < maxCount; i++)
            {
                if (!seriesHig.ContainsKey(i))
                {
                    seriesHig[i] = 0;
                }
                float value = serie.yData[i];
                float pY = startY + i * scaleWid;
                float pX = seriesHig[i] + coordinateX + m_Coordinate.tickness;
                float dataHig = (value - xAxis.minValue) / (xAxis.maxValue - xAxis.minValue) * coordinateWid;
                np = new Vector3(pX + dataHig, pY);

                if (i > 0)
                {
                    if (m_Line.step)
                    {
                        Vector2 middle1, middle2;
                        switch (m_Line.stepTpe)
                        {
                            case Line.StepType.Start:
                                middle1 = new Vector2(np.x, lp.y);
                                middle2 = new Vector2(np.x, lp.y - m_Line.tickness);
                                ChartHelper.DrawLine(vh, lp, middle1, m_Line.tickness, color);
                                ChartHelper.DrawLine(vh, middle2, np, m_Line.tickness, color);
                                if (m_Line.area)
                                {
                                    Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                                    ChartHelper.DrawPolygon(vh, new Vector2(coordinateX, middle1.y), middle1, np,
                                        new Vector2(coordinateX, np.y), areaColor);
                                }
                                break;
                            case Line.StepType.Middle:
                                middle1 = new Vector2(lp.x, (lp.y + np.y) / 2 + m_Line.tickness);
                                middle2 = new Vector2(np.x, (lp.y + np.y) / 2 - m_Line.tickness);
                                ChartHelper.DrawLine(vh, lp, middle1, m_Line.tickness, color);
                                ChartHelper.DrawLine(vh, new Vector2(middle1.x, middle1.y - m_Line.tickness),
                                    new Vector2(middle2.x, middle2.y + m_Line.tickness), m_Line.tickness, color);
                                ChartHelper.DrawLine(vh, middle2, np, m_Line.tickness, color);
                                if (m_Line.area)
                                {
                                    Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                                    ChartHelper.DrawPolygon(vh, new Vector2(coordinateX, lp.y), lp, middle1,
                                        new Vector2(coordinateX, middle1.y), areaColor);
                                    ChartHelper.DrawPolygon(vh, new Vector2(coordinateX, middle2.y + 2 * m_Line.tickness),
                                        new Vector2(middle2.x, middle2.y + 2 * m_Line.tickness), np,
                                        new Vector2(coordinateX, np.y), areaColor);
                                }
                                break;
                            case Line.StepType.End:
                                middle1 = new Vector2(np.x, lp.y);
                                middle2 = new Vector2(np.x, lp.y - m_Line.tickness);
                                ChartHelper.DrawLine(vh, lp, middle1, m_Line.tickness, color);
                                ChartHelper.DrawLine(vh, middle2, np, m_Line.tickness, color);
                                if (m_Line.area)
                                {
                                    Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                                    ChartHelper.DrawPolygon(vh, new Vector2(coordinateX, lp.y), middle1,
                                        new Vector2(np.x, np.y),
                                        new Vector2(coordinateX, np.y), areaColor);
                                }
                                break;
                        }
                    }
                    else if (m_Line.smooth)
                    {
                        var list = ChartHelper.GetBezierListVertical(lp, np, m_Line.smoothStyle);
                        Vector3 start, to;
                        start = list[0];
                        for (int k = 1; k < list.Length; k++)
                        {
                            smoothPoints.Add(list[k]);
                            to = list[k];
                            ChartHelper.DrawLine(vh, start, to, m_Line.tickness, color);

                            if (m_Line.area)
                            {
                                Vector3 alp = new Vector3(start.x, start.y - m_Line.tickness);
                                Vector3 anp = new Vector3(to.x, to.y - m_Line.tickness);
                                Vector3 tnp = serieIndex > 0 ?
                                    (smoothPointCount > lastSmoothPoints.Count - 1 ?
                                    new Vector3(lastSmoothPoints[lastSmoothPoints.Count - 1].x,
                                        lastSmoothPoints[lastSmoothPoints.Count - 1].y + m_Line.tickness) :
                                    new Vector3(lastSmoothPoints[smoothPointCount].x,
                                        lastSmoothPoints[smoothPointCount].y + m_Line.tickness)) :
                                    new Vector3(coordinateX + m_Coordinate.tickness, to.y);
                                Vector3 tlp = serieIndex > 0 ?
                                    (smoothPointCount > lastSmoothPoints.Count - 1 ?
                                    new Vector3(lastSmoothPoints[lastSmoothPoints.Count - 2].x,
                                        lastSmoothPoints[lastSmoothPoints.Count - 2].y + m_Line.tickness) :
                                    new Vector3(lastSmoothPoints[smoothPointCount - 1].x,
                                        lastSmoothPoints[smoothPointCount - 1].y + m_Line.tickness)) :
                                    new Vector3(coordinateX + m_Coordinate.tickness, start.y);
                                Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                                ChartHelper.DrawPolygon(vh, alp, anp, tnp, tlp, areaColor);
                            }
                            smoothPointCount++;
                            start = to;
                        }
                    }
                    else
                    {
                        ChartHelper.DrawLine(vh, lp, np, m_Line.tickness, color);
                        if (m_Line.area)
                        {
                            Vector3 alp = new Vector3(lp.x, lp.y - m_Line.tickness);
                            Vector3 anp = new Vector3(np.x, np.y - m_Line.tickness);
                            Color areaColor = new Color(color.r, color.g, color.b, color.a * 0.75f);
                            var cross = ChartHelper.GetIntersection(lp, np, new Vector3(coordinateX, coordinateY),
                                new Vector3(coordinateX, coordinateY + coordinateHig));
                            if (cross == Vector3.zero)
                            {
                                Vector3 tnp = serieIndex > 0 ?
                                    new Vector3(lastPoints[i].x, lastPoints[i].y + m_Line.tickness) :
                                    new Vector3(coordinateX + m_Coordinate.tickness, np.y);
                                Vector3 tlp = serieIndex > 0 ?
                                    new Vector3(lastPoints[i - 1].x, lastPoints[i - 1].y + m_Line.tickness) :
                                    new Vector3(coordinateX + m_Coordinate.tickness, lp.y);
                                ChartHelper.DrawPolygon(vh, alp, anp, tnp, tlp, areaColor);
                            }
                            else
                            {
                                Vector3 cross1 = new Vector3(cross.x + (alp.x > coordinateX ? m_Coordinate.tickness : -m_Coordinate.tickness), cross.y);
                                Vector3 cross2 = new Vector3(cross.x + (anp.x > coordinateX ? m_Coordinate.tickness : -m_Coordinate.tickness), cross.y);
                                Vector3 xp1 = new Vector3(coordinateX + (alp.x > coordinateX ? m_Coordinate.tickness : -m_Coordinate.tickness), alp.y);
                                Vector3 xp2 = new Vector3(coordinateX + (anp.x > coordinateX ? m_Coordinate.tickness : -m_Coordinate.tickness), anp.y);
                                ChartHelper.DrawTriangle(vh, alp, cross1, xp1, areaColor);
                                ChartHelper.DrawTriangle(vh, anp, cross2, xp2, areaColor);
                            }
                        }
                    }
                }
                if (m_Line.point)
                {
                    points.Add(np);
                    colors.Add(color);
                }
                seriesHig[i] += dataHig;
                lp = np;
            }
        }
    }
}
