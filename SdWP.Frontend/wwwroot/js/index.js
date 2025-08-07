window.initializeDataTable = function () {
    const table = window.projectsDataTable.initialize('#projectsViewTable');
    window.projectsEvents.register('#projectsViewTable', table);

    $(document).ready(function () {
        if (window.resetPassword && typeof window.resetPassword.register === 'function') {
            window.resetPassword.register();
        }
    });

    table.on('draw.dt', function () {
        $('#projectsViewTable td.sorting_1').removeClass('sorting_1');
    });
};