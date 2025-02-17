﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace XCharts
{
    public enum Orient
    {
        Horizonal,
        Vertical
    }

    public class BaseChart : MaskableGraphic, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler,
        IDragHandler, IEndDragHandler, IScrollHandler
    {
        private static readonly string s_TitleObjectName = "title";
        private static readonly string s_LegendObjectName = "legend";

        [SerializeField] protected float m_ChartWidth;
        [SerializeField] protected float m_ChartHeight;
        [SerializeField] protected Theme m_Theme = Theme.Default;
        [SerializeField] protected ThemeInfo m_ThemeInfo;
        [SerializeField] protected Title m_Title = Title.defaultTitle;
        [SerializeField] protected Legend m_Legend = Legend.defaultLegend;
        [SerializeField] protected Tooltip m_Tooltip = Tooltip.defaultTooltip;
        [SerializeField] protected Series m_Series = Series.defaultSeries;

        [SerializeField] protected bool m_Large;
        [SerializeField] protected int m_MinShowDataNumber;
        [SerializeField] protected int m_MaxShowDataNumber;
        [SerializeField] protected int m_MaxCacheDataNumber;

        [NonSerialized] private Theme m_CheckTheme = 0;
        [NonSerialized] private Title m_CheckTitle = Title.defaultTitle;
        [NonSerialized] private Legend m_CheckLegend = Legend.defaultLegend;
        [NonSerialized] private float m_CheckWidth = 0;
        [NonSerialized] private float m_CheckHeight = 0;
        [NonSerialized] private bool m_RefreshChart = false;
        [NonSerialized] protected List<Text> m_LegendTextList = new List<Text>();

        protected Vector2 chartAnchorMax { get { return rectTransform.anchorMax; } }
        protected Vector2 chartAnchorMin { get { return rectTransform.anchorMin; } }
        protected Vector2 chartPivot { get { return rectTransform.pivot; } }

        public Title title { get { return m_Title; } }
        public Legend legend { get { return m_Legend; } }
        public Tooltip tooltip { get { return m_Tooltip; } }
        public Series series { get { return m_Series; } }

        public float chartWidth { get { return m_ChartWidth; } }
        public float chartHeight { get { return m_ChartHeight; } }

        /// <summary>
        /// The min number of data to show in chart.
        /// </summary>
        public int minShowDataNumber
        {
            get { return m_MinShowDataNumber; }
            set { m_MinShowDataNumber = value; if (m_MinShowDataNumber < 0) m_MinShowDataNumber = 0; }
        }

        /// <summary>
        /// The max number of data to show in chart.
        /// </summary>
        public int maxShowDataNumber
        {
            get { return m_MaxShowDataNumber; }
            set { m_MaxShowDataNumber = value; if (m_MaxShowDataNumber < 0) m_MaxShowDataNumber = 0; }
        }

        /// <summary>
        /// The max number of serie and axis data cache.
        /// The first data will be remove when the size of serie and axis data is larger then maxCacheDataNumber.
        /// default:0,unlimited.
        /// </summary>
        public int maxCacheDataNumber
        {
            get { return m_MaxCacheDataNumber; }
            set { m_MaxCacheDataNumber = value; if (m_MaxCacheDataNumber < 0) m_MaxCacheDataNumber = 0; }
        }

        /// <summary>
        /// Set the size of chart.
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        public virtual void SetSize(float width, float height)
        {
            m_ChartWidth = width;
            m_ChartHeight = height;
            m_CheckWidth = width;
            m_CheckHeight = height;
            rectTransform.sizeDelta = new Vector2(m_ChartWidth, m_ChartHeight);
            OnSizeChanged();
        }

        /// <summary>
        /// Remove all series and legend data.
        /// It just emptying all of serie's data without emptying the list of series.
        /// </summary>
        public virtual void ClearData()
        {
            m_Series.ClearData();
            m_Legend.ClearData();
            RefreshChart();
        }

        /// <summary>
        /// Remove legend and serie by name.
        /// </summary>
        /// <param name="serieName">the name of serie</param>
        public virtual void RemoveData(string serieName)
        {
            m_Series.Remove(serieName);
            m_Legend.RemoveData(serieName);
            RefreshChart();
        }

        /// <summary>
        /// Remove all data from series and legend.
        /// The series list is also cleared.
        /// </summary>
        public virtual void RemoveData()
        {
            m_Legend.ClearData();
            m_Series.RemoveAll();
            RefreshChart();
        }

        /// <summary>
        /// Add a serie to serie list.
        /// </summary>
        /// <param name="serieName">the name of serie</param>
        /// <param name="type">the type of serie</param>
        /// <param name="show">whether to show this serie</param>
        /// <returns>the added serie</returns>
        public virtual Serie AddSerie(string serieName, SerieType type, bool show = true)
        {
            m_Legend.AddData(serieName);
            return m_Series.AddSerie(serieName, type);
        }

        /// <summary>
        /// Add a data to serie.
        /// If serie doesn't exist,will be add to series.
        /// If serieName doesn't exist in legend,will be add to legend.
        /// </summary>
        /// <param name="serieName">the name of serie</param>
        /// <param name="value">the data to add</param>
        public virtual void AddData(string serieName, float value)
        {
            m_Legend.AddData(serieName);
            m_Series.AddData(serieName, value, m_MaxCacheDataNumber);
            RefreshChart();
        }

        /// <summary>
        /// Add a data to serie.
        /// If serie doesn't exist, the data is ignored.
        /// </summary>
        /// <param name="serieIndex">the index of serie</param>
        /// <param name="value">the data to add</param>
        public virtual void AddData(int serieIndex, float value)
        {
            m_Series.AddData(serieIndex, value, m_MaxCacheDataNumber);
            RefreshChart();
        }

        /// <summary>
        /// Update serie data by serie name.
        /// </summary>
        /// <param name="serieName">the name of serie</param>
        /// <param name="value">the data will be update</param>
        /// <param name="dataIndex">the index of data</param>
        public virtual void UpdateData(string serieName, float value, int dataIndex = 0)
        {
            m_Series.UpdateData(serieName, value, dataIndex);
            RefreshChart();
        }

        /// <summary>
        /// Update serie data by serie index.
        /// </summary>
        /// <param name="serieIndex">the index of serie</param>
        /// <param name="value">the data will be update</param>
        /// <param name="dataIndex">the index of data</param>
        public virtual void UpdateData(int serieIndex, float value, int dataIndex = 0)
        {
            m_Series.UpdateData(serieIndex, value, dataIndex);
            RefreshChart();
        }

        /// <summary>
        /// Whether to show serie and legend.
        /// </summary>
        /// <param name="serieName">the name of serie</param>
        /// <param name="active">Active or not</param>
        public virtual void SetActive(string serieName, bool active)
        {
            m_Legend.SetActive(serieName, active);
            m_Series.SetActive(serieName, active);
        }

        /// <summary>
        /// Whether to show serie and legend.
        /// </summary>
        /// <param name="serieIndex">the index of serie</param>
        /// <param name="active">Active or not</param>
        public virtual void SetActive(int serieIndex, bool active)
        {
            m_Legend.SetActive(serieIndex, active);
            m_Series.SetActive(serieIndex, active);
        }

        /// <summary>
        /// Whether serie is activated.
        /// </summary>
        /// <param name="serieName">the name of serie</param>
        /// <returns>True when activated</returns>
        public virtual bool IsActive(string serieName)
        {
            return m_Legend.IsActive(serieName) || m_Series.IsActive(serieName);
        }

        /// <summary>
        /// Whether serie is activated.
        /// </summary>
        /// <param name="serieIndex">the index of serie</param>
        /// <returns>True when activated</returns>
        public virtual bool IsActive(int serieIndex)
        {
            return m_Legend.IsActive(serieIndex) || m_Series.IsActive(serieIndex);
        }

        /// <summary>
        /// Redraw chart next frame.
        /// </summary>
        public void RefreshChart()
        {
            m_RefreshChart = true;
        }

        /// <summary>
        /// Update chart theme
        /// </summary>
        /// <param name="theme">theme</param>
        public void UpdateTheme(Theme theme)
        {
            this.m_Theme = theme;
            OnThemeChanged();
            RefreshChart();
        }

        protected override void Awake()
        {
            if (m_ThemeInfo == null)
            {
                m_ThemeInfo = ThemeInfo.Default;
            }
            raycastTarget = false;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            m_ChartWidth = rectTransform.sizeDelta.x;
            m_ChartHeight = rectTransform.sizeDelta.y;
            m_CheckWidth = m_ChartWidth;
            m_CheckHeight = m_ChartHeight;
            m_CheckTheme = m_Theme;
            InitTitle();
            InitLegend();
            InitTooltip();
        }

        protected virtual void Update()
        {
            CheckSize();
            CheckTheme();
            CheckTile();
            CheckLegend();
            CheckTooltip();
            CheckRefreshChart();
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            var sizeDelta = rectTransform.sizeDelta;
            if (sizeDelta.x < 580 && sizeDelta.y < 300)
            {
                rectTransform.sizeDelta = new Vector2(580, 300);
            }
            ChartHelper.DestoryAllChilds(transform);
            m_ThemeInfo = ThemeInfo.Default;
            m_Title = Title.defaultTitle;
            m_Legend = Legend.defaultLegend;
            m_Tooltip = Tooltip.defaultTooltip;
            m_Series = Series.defaultSeries;
            Awake();
        }
#endif

        protected override void OnDestroy()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private void InitTitle()
        {
            m_Title.OnChanged();
            TextAnchor anchor = m_Title.location.textAnchor;
            Vector2 anchorMin = m_Title.location.anchorMin;
            Vector2 anchorMax = m_Title.location.anchorMax;
            Vector2 pivot = m_Title.location.pivot;
            Vector3 titlePosition = m_Title.location.GetPosition(chartWidth, chartHeight);
            Vector3 subTitlePosition = -new Vector3(0, m_Title.textFontSize + m_Title.itemGap, 0);
            float titleWid = chartWidth;

            var titleObject = ChartHelper.AddObject(s_TitleObjectName, transform, anchorMin, anchorMax,
                pivot, new Vector2(chartWidth, chartHeight));
            titleObject.transform.localPosition = titlePosition;
            ChartHelper.HideAllObject(titleObject, s_TitleObjectName);

            Text titleText = ChartHelper.AddTextObject(s_TitleObjectName, titleObject.transform,
                        m_ThemeInfo.font, m_ThemeInfo.textColor, anchor, anchorMin, anchorMax, pivot,
                        new Vector2(titleWid, m_Title.textFontSize), m_Title.textFontSize);

            titleText.alignment = anchor;
            titleText.gameObject.SetActive(m_Title.show);
            titleText.transform.localPosition = Vector2.zero;
            titleText.text = m_Title.text;

            Text subText = ChartHelper.AddTextObject(s_TitleObjectName + "_sub", titleObject.transform,
                        m_ThemeInfo.font, m_ThemeInfo.textColor, anchor, anchorMin, anchorMax, pivot,
                        new Vector2(titleWid, m_Title.subTextFontSize), m_Title.subTextFontSize);

            subText.alignment = anchor;
            subText.gameObject.SetActive(m_Title.show && !string.IsNullOrEmpty(m_Title.subText));
            subText.transform.localPosition = subTitlePosition;
            subText.text = m_Title.subText;
        }

        private void InitLegend()
        {
            m_Legend.OnChanged();
            ChartHelper.HideAllObject(transform, s_LegendObjectName);
            TextAnchor anchor = m_Legend.location.textAnchor;
            Vector2 anchorMin = m_Legend.location.anchorMin;
            Vector2 anchorMax = m_Legend.location.anchorMax;
            Vector2 pivot = m_Legend.location.pivot;

            var legendObject = ChartHelper.AddObject(s_LegendObjectName, transform, anchorMin, anchorMax,
                pivot, new Vector2(chartWidth, chartHeight));
            legendObject.transform.localPosition = m_Legend.location.GetPosition(chartWidth, chartHeight);
            ChartHelper.HideAllObject(legendObject, s_LegendObjectName);

            for (int i = 0; i < m_Legend.data.Count; i++)
            {
                Button btn = ChartHelper.AddButtonObject(s_LegendObjectName + "_" + i, legendObject.transform,
                    m_ThemeInfo.font, m_Legend.itemFontSize, m_ThemeInfo.legendTextColor, anchor,
                    anchorMin, anchorMax, pivot, new Vector2(m_Legend.itemWidth, m_Legend.itemHeight));

                m_Legend.SetButton(i, btn);
                m_Legend.SetActive(i, IsActive(i));
                m_Legend.UpdateButtonColor(i, m_ThemeInfo.GetColor(i), m_ThemeInfo.legendUnableColor);
                btn.GetComponentInChildren<Text>().text = m_Legend.data[i];
                ChartHelper.AddEventListener(btn.gameObject, EventTriggerType.PointerDown, (data) =>
                {
                    int count = (data as PointerEventData).clickCount;
                    int index = int.Parse(data.selectedObject.name.Split('_')[1]);
                    SetActive(index, !m_Legend.IsActive(index));
                    m_Legend.UpdateButtonColor(index, m_ThemeInfo.GetColor(index),
                        m_ThemeInfo.legendUnableColor);
                    OnYMaxValueChanged();
                    OnLegendButtonClicked();
                    RefreshChart();
                });
            }
        }

        private void InitTooltip()
        {
            var tooltipObject = ChartHelper.AddObject("tooltip", transform, chartAnchorMin,
                chartAnchorMax, chartPivot, new Vector2(chartWidth, chartHeight));
            tooltipObject.transform.localPosition = Vector3.zero;
            DestroyImmediate(tooltipObject.GetComponent<Image>());
            var parent = tooltipObject.transform;
            ChartHelper.HideAllObject(tooltipObject.transform);
            GameObject content = ChartHelper.AddTooltipContent("content", parent, m_ThemeInfo.font);
            m_Tooltip.SetObj(tooltipObject);
            m_Tooltip.SetContentObj(content);
            m_Tooltip.SetContentBackgroundColor(m_ThemeInfo.tooltipBackgroundColor);
            m_Tooltip.SetContentTextColor(m_ThemeInfo.tooltipTextColor);
            m_Tooltip.SetActive(false);
        }

        private Vector3 GetLegendPosition(int i)
        {
            return m_Legend.location.GetPosition(chartWidth, chartHeight);
        }

        protected float GetMaxValue(int index)
        {
            return m_Series.GetMaxValue(index);
        }

        private void CheckSize()
        {
            if (m_CheckWidth != chartWidth || m_CheckHeight != chartHeight)
            {
                SetSize(chartWidth, chartHeight);
            }
            var sizeDelta = rectTransform.sizeDelta;
            if (m_CheckWidth != sizeDelta.x || m_CheckHeight != sizeDelta.y)
            {
                SetSize(sizeDelta.x, sizeDelta.y);
            }
        }

        private void CheckTheme()
        {
            if (m_CheckTheme != m_Theme)
            {
                m_CheckTheme = m_Theme;
                OnThemeChanged();
            }
        }

        private void CheckTile()
        {
            if (!m_CheckTitle.Equals(m_Title))
            {
                m_CheckTitle.Copy(m_Title);
                OnTitleChanged();
            }
        }

        private void CheckLegend()
        {
            if (m_CheckLegend != m_Legend)
            {
                m_CheckLegend.Copy(m_Legend);
                OnLegendChanged();
            }
        }

        private void CheckTooltip()
        {
            if (!m_Tooltip.show || !m_Tooltip.isInited)
            {
                if (m_Tooltip.dataIndex != 0)
                {
                    m_Tooltip.dataIndex = 0;
                    m_Tooltip.SetActive(false);
                    RefreshChart();
                }
                return;
            }
            m_Tooltip.dataIndex = 0;

            Vector2 local;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                Input.mousePosition, canvas.worldCamera, out local))
            {
                m_Tooltip.SetActive(false);
                RefreshChart();
                return;
            }
            if (local.x < 0 || local.x > chartWidth ||
                local.y < 0 || local.y > chartHeight)
            {
                m_Tooltip.SetActive(false);
                RefreshChart();
                return;
            }
            m_Tooltip.pointerPos = local;
            CheckTootipArea(local);
        }

        protected virtual void CheckTootipArea(Vector2 localPostion)
        {
        }

        protected void CheckRefreshChart()
        {
            if (m_RefreshChart)
            {
                int tempWid = (int)chartWidth;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempWid - 1);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempWid);
                m_RefreshChart = false;
            }
        }

        protected virtual void OnSizeChanged()
        {
            InitTitle();
            InitLegend();
            InitTooltip();
        }

        protected virtual void OnThemeChanged()
        {
            switch (m_Theme)
            {
                case Theme.Dark:
                    m_ThemeInfo.Copy(ThemeInfo.Dark);
                    break;
                case Theme.Default:
                    m_ThemeInfo.Copy(ThemeInfo.Default);
                    break;
                case Theme.Light:
                    m_ThemeInfo.Copy(ThemeInfo.Light);
                    break;
            }
            InitTitle();
            InitLegend();
            InitTooltip();
        }

        protected virtual void OnTitleChanged()
        {
            InitTitle();
        }

        protected virtual void OnLegendChanged()
        {
            InitLegend();
        }

        protected virtual void OnYMaxValueChanged()
        {
        }

        protected virtual void OnLegendButtonClicked()
        {
        }

        protected virtual void RefreshTooltip()
        {
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            DrawBackground(vh);
            DrawChart(vh);
            DrawTooltip(vh);
        }

        protected virtual void DrawChart(VertexHelper vh)
        {
        }

        protected virtual void DrawTooltip(VertexHelper vh)
        {
        }

        private void DrawBackground(VertexHelper vh)
        {
            // draw bg
            Vector3 p1 = new Vector3(0, chartHeight);
            Vector3 p2 = new Vector3(chartWidth, chartHeight);
            Vector3 p3 = new Vector3(chartWidth, 0);
            Vector3 p4 = new Vector3(0, 0);
            ChartHelper.DrawPolygon(vh, p1, p2, p3, p4, m_ThemeInfo.backgroundColor);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {

        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
        }
    }
}
