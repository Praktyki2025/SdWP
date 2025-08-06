let selectedUserId = null;

window.usersEvents = {
    register: function (tableSelector, tableInstance) {
        $(tableSelector).on('click', '.edit-user', function () {
            const id = $(this).data('id');
            window.dispatchEvent(new CustomEvent('userEditModal', { detail: id }));
        });

        $(tableSelector).on('click', '.delete-user', function () {
            selectedUserId = $(this).data('id');
            const modal = new bootstrap.Modal(document.getElementById('deleteUserModal'));
            modal.show();
        });

        $('#confirmDeleteUserButton').on('click', async function () {
            if (!selectedUserId) return;

            try {
                await window.userApi.deleteUser(selectedUserId);
                tableInstance.ajax.reload(null, false);
            } catch (err) {
                alert(`Error deleting user: ${err.message}`);
            }

            const modalEl = document.getElementById('deleteUserModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            modalInstance.hide();

            selectedUserId = null;
        });
    }
};

window.usersModals = {
    init: function () {
        $('#registerUserForm').on('submit', async function (e) {
            e.preventDefault();
            const data = {
                name: $('#registerName').val(),
                email: $('#registerEmail').val(),
                password: $('#registerPassword').val(),
                confirmPassword: $('#registerConfirmPassword').val(),
                role: $('#registerRole').val()
            };

            try {
                await window.userApi.registerUser(data);
                $('#registerUserModal').modal('hide');
                window.initializeUserTable();
            } catch (err) {
                alert(`Error registering user: ${err.message}`);
            }
        });

        window.addEventListener('userEditModal', async (e) => {
            const userId = e.detail;
            const rowData = $('#userTable').DataTable().rows().data().toArray().find(x => x.id === userId);

            if (!rowData) return;

            $('#editUserId').val(rowData.id);
            $('#editName').val(rowData.name);
            $('#editEmail').val(rowData.email);
            $('#editPassword').val('');
            $('#editConfirmPassword').val('');
            $('#editRole').val(rowData.roles?.[0] ?? '');

            const modal = new bootstrap.Modal(document.getElementById('editUserModal'));
            modal.show();
        });

        $('#editUserForm').on('submit', async function (e) {
            e.preventDefault();

            const data = {
                id: $('#editUserId').val(),
                name: $('#editName').val(),
                email: $('#editEmail').val(),
                password: $('#editPassword').val(),
                confirmPassword: $('#editConfirmPassword').val(),
                role: $('#editRole').val()
            };

            try {
                await window.userApi.updateUser(data);
                $('#editUserModal').modal('hide');
                window.initializeUserTable();
            } catch (e) {
                $('#editUserMessage').removeClass('d-none alert-success').addClass('alert-danger').text(e.message);
            }
        });
    }
};

window.showRegisterModal = function () {
    const modal = new bootstrap.Modal(document.getElementById('registerUserModal'));
    modal.show();
};
