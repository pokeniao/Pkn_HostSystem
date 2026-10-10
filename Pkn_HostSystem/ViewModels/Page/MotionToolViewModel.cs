using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic.Logging;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class MotionToolViewModel :ObservableRecipient
    {
        public LogControl<MotionToolViewModel> Log { get; set; }

        public MotionToolModel MotionToolModel { get; set; } = new();
        public ISnackbarService SnackbarService { get; set; }
        public MotionToolViewModel()
        {
            SnackbarService = new SnackbarService();
            Log = new LogControl<MotionToolViewModel>(SnackbarService);
        }

        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

    }
}
