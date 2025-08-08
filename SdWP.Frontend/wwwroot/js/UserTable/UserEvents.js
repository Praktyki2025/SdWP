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
                const modalEl = document.getElementById('registerUserModal');
                const modalInstance = bootstrap.Modal.getInstance(modalEl);

                if (modalInstance) {
                    modalInstance.hide();
                }

                setTimeout(() => {
                    const table = $('#userTable').DataTable();
                    if (table) {
                        table.ajax.reload(null, false);
                    }
                }, 300);

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
                const modalEl = document.getElementById('editUserModal');
                const modalInstance = bootstrap.Modal.getInstance(modalEl);

                if (modalInstance) modalInstance.hide();

                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');

                setTimeout(() => {
                    const table = $('#userTable').DataTable();
                    if (table) {
                        table.ajax.reload(null, false);
                    }
                }, 300);

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
