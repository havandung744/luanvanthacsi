using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using Microsoft.AspNetCore.Components;
using luanvanthacsi.Data;
using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using AntDesign;
using Microsoft.AspNetCore.Components.Web;
using luanvanthacsi.Data.Extentions;
using Tewr.Blazor.FileReader;
using Microsoft.JSInterop;
using OfficeOpenXml.Style.XmlAccess;
using static luanvanthacsi.Data.Components.Enum;
using AutoMapper;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Ultils;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class EvaluationBoardEdit : ComponentBase
    {
        [Inject] IEvaluationBoardService EvaluationBoardService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [Parameter] public User CurrentUser { get; set; }
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<EvaluationBoard> ValueChange { get; set; }
        EvaluationBoardEditModel EditModel { get; set; } = new EvaluationBoardEditModel();
        ElementReference inputTypeFileElement;
        EvaluationBoardEditModel oldEditmode { get; set; }
        string Id { get; set; }
        protected override void OnInitialized()
        {
            oldEditmode = EditModel;
        }
        public void LoadData(EvaluationBoard evaluationBoard)
        {
            EditModel = _mapper.Map<EvaluationBoardEditModel>(evaluationBoard);
            StateHasChanged();
        }

        public void UpdateEvaluationBoard()
        {
            EvaluationBoard evaluationBoard = _mapper.Map<EvaluationBoard>(EditModel);
            evaluationBoard.FacultyId = CurrentUser.FacultyId;
            ValueChange.InvokeAsync(evaluationBoard);
        }

        private void OnFinish(EditContext editContext)
        {
            UpdateEvaluationBoard();
        }

        private void OnFinishFailed(EditContext editContext)
        {
            //EditModel = new ScientistEditModel();
        }

        public void Close()
        {
            EditModel = new EvaluationBoardEditModel();
            StateHasChanged();
        }
        private async Task Reset(MouseEventArgs args)
        {
            if (EditModel.Id == null)
            {
                var newCode = EditModel.Code;
                EditModel = new EvaluationBoardEditModel();
                EditModel.Code = newCode;
            }
            else
            {
                EvaluationBoard oldEditmodel = await EvaluationBoardService.GetEvaluationBoardByIdAsync(EditModel.Id);
                EditModel = _mapper.Map<EvaluationBoardEditModel>(oldEditmodel);
            }
            StateHasChanged();
        }
      
    }
}
