window.initDataTable = {
    initialize: function (tableId) {
        setTimeout(() => {
            const table = document.getElementById(tableId);
            if (!table) {
                return;
            }

            if ($.fn.dataTable.isDataTable('#' + tableId)) {
                $('#' + tableId).DataTable().destroy();
            }

            $('#' + tableId).DataTable({
                responsive: true
            });
        }, 100);
    },
    destroy: function (tableId) {
        let table = $('#' + tableId).DataTable()
        if (table) table.destroy();
    }
};