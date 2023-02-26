using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using System.Linq;
using AntDesign;
using luanvanthacsi.Data.Components;
using FluentNHibernate.Conventions;
using luanvanthacsi.Data.Extentions;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components.Authorization;
using AutoMapper;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class EvaluationBoardList : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IEvaluationBoardService EvaluationBoardService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        List<EvaluationBoardData>? evaluationBoardDatas { get; set; }
        bool visible = false;
        EvaluationBoardEdit evaluationBoardEdit = new EvaluationBoardEdit();
        bool loading = false;
        IEnumerable<EvaluationBoardData>? selectedRows;
        EvaluationBoardData? selectData;
        Table<EvaluationBoardData>? table;
        List<string>? ListSelectedEvaluationBoardDataIds;
        User CurrentUser;
        protected override async Task OnInitializedAsync()
        {
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);
            evaluationBoardDatas = new();
            await LoadAsync();
        }
        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }

        public async Task LoadAsync()
        {
            evaluationBoardDatas.Clear();
            loading = true;
            visible = false;
            StateHasChanged();
            var evaluationBoards = await EvaluationBoardService.GetAllByIdAsync(CurrentUser.FacultyId);
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = evaluationBoards.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            evaluationBoardDatas = _mapper.Map<List<EvaluationBoardData>>(list);
            int stt = 1;
            evaluationBoardDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
        }

        void AddEvaluationBoard()
        {
            var evaluationBoardData = new EvaluationBoard();
            var lastCode = evaluationBoardDatas?.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();
            int codeNumber = 0;
            if (lastCode != null && int.TryParse(lastCode.Substring(4), out codeNumber))
            {
                codeNumber++;
            }
            string newCode = "HDDG" + codeNumber.ToString("D3");
            evaluationBoardData.Code = newCode;
            ShowEvaluationBoardDetail(evaluationBoardData);
        }

        void ShowEvaluationBoardDetail(EvaluationBoard data)
        {
            evaluationBoardEdit.LoadData(data);
            visible = true;
        }

        async Task Save(EvaluationBoard data)
        {
            var resultAdd = await EvaluationBoardService.AddOrUpdateEvaluationBoard(data);
            await LoadAsync();
            if (data.Id.IsNotNullOrEmpty())
            {
                Notice.NotiSuccess("Cập nhật dữ liệu thành công");
            }
            else
            {
                Notice.NotiSuccess("Thêm dữ liệu thành công");
            }
        }

        void OnClose()
        {
            evaluationBoardEdit.Close();
            visible = false;
        }

        async Task Edit(EvaluationBoardData EvaluationBoardData)
        {
            EvaluationBoard evaluationBoard = await EvaluationBoardService.GetEvaluationBoardByIdAsync(EvaluationBoardData.Id);
            ShowEvaluationBoardDetail(evaluationBoard);
        }



        async Task DeleteEvaluationBoard(EvaluationBoardData evaluationBoardData)
        {
            EvaluationBoard evaluationBoard = await EvaluationBoardService.GetEvaluationBoardByIdAsync(evaluationBoardData.Id.ToString());
            var result = await EvaluationBoardService.DeleteEvaluationBoardAsync(evaluationBoard);
            if (result.Equals(true))
            {
                Notice.NotiSuccess("Xóa dữ liệu thành công");
                await LoadAsync();
            }
            else
            {
                Notice.NotiError("Xóa dữ liệu thất bại");
            }
        }

        void OnRowClick(RowData<EvaluationBoardData> rowData)
        {
            try
            {
                List<string> ids;
                selectData = evaluationBoardDatas?.FirstOrDefault(c => c.Id == rowData.Data.Id);
                ids = selectedRows != null ? selectedRows.Select(c => c.Id).ToList() : new();
                if (ids.Contains(selectData.Id))
                {
                    ids.Remove(selectData.Id);
                }
                else
                {
                    ids.Add(selectData.Id);
                }
                table?.SetSelection(ids.ToArray());
                ListSelectedEvaluationBoardDataIds = ids;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task DeleteAsync(EvaluationBoardData model = null)
        {
            try
            {
                var deleteModel = evaluationBoardDatas.Where(c => selectedRows.Select(r => r.Id).Contains(c.Id)).ToList();
                List<EvaluationBoard> evaluationBoards = new List<EvaluationBoard>();
                // maping từ studentData thành student
                foreach (var evaluationBoardData in deleteModel)
                {
                    EvaluationBoard evaluationBoard = new EvaluationBoard();
                    evaluationBoard.Id = evaluationBoardData.Id;
                    evaluationBoards.Add(evaluationBoard);
                }
                var result = await EvaluationBoardService.DeleteEvaluationBoardAsync(evaluationBoards);
                if (result.Equals(true))
                {
                    Notice.NotiSuccess("Xóa dữ liệu thành công");
                    await LoadAsync();
                }
                else
                {
                    Notice.NotiError("Xóa dữ liệu thất bại");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
