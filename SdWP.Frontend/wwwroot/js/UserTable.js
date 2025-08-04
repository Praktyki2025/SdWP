window.initDataTable = {
    initialize: function (tableId) {
        setTimeout(() => {
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