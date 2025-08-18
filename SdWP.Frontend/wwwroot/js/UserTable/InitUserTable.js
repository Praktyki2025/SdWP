window.initializeUserTable = function () {
    const table = window.userDataTable.init('#userTable');
    window.usersEvents.register('#userTable', table);

    table.on('draw.dt', function () {
        $('#userTable td.sorting_1').removeClass('sorting_1');
    });
};


window.destroyUserTable = function () {
    try {
        if ($.fn.dataTable.isDataTable('#userTable')) {
            $('#userTable').DataTable().clear();
            $('#userTable').DataTable().destroy();
        }

        $('#userTable').off('click', '.edit-user');
        $('#userTable').off('click', '.delete-user');
        $('#confirmDeleteUserButton').off('click');
        $('#registerUserForm').off('submit');
        $('#editUserForm').off('submit');

        window.removeEventListener('userEditModal', arguments.callee);

        $('.dataTables_wrapper').remove();
        $('.dataTables_paginate').remove();
        $('.dataTables_filter').remove();
        $('.dataTables_length').remove();
        $('.dataTables_info').remove();

        $('.modal.show').modal('hide');
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');

        if (typeof selectedUserId !== 'undefined') {
            selectedUserId = null;
        }

    } catch (error) {
        console.log('Error during DataTable cleanup:', error);
    }
};

window.cleanupAllDataTables = function () {
    $('table').each(function () {
        if ($.fn.dataTable.isDataTable(this)) {
            $(this).DataTable().destroy();
        }
    });

    $('.dataTables_wrapper').remove();
    $('.dataTables_paginate').remove();
    $('.dataTables_filter').remove();
    $('.dataTables_length').remove();
    $('.dataTables_info').remove();

    $('.modal.show').modal('hide');
    $('.modal-backdrop').remove();
    $('body').removeClass('modal-open');
};