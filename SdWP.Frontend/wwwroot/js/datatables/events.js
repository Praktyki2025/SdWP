let selectedProjectId = null;

window.projectsEvents = {
    register: function (tableSelector, tableInstance) {
        $(tableSelector).on('click', '.edit-project', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            window.location.href = `/projects/edit?id=${id}`;
        });

        $(tableSelector).on('click', '.delete-project', function (e) {
            e.preventDefault();
            selectedProjectId = $(this).data('id');
            const myModal = new bootstrap.Modal(document.getElementById('myModal'));
            myModal.show();
        });

        $('#confirmButton').on('click', async function () {
            if (selectedProjectId === null) return;

            try {
                await window.projectsApi.deleteProject(selectedProjectId);
                tableInstance.ajax.reload(null, false);
            } catch {
                alert('Failed to delete project');
            }

            const modalEl = document.getElementById('myModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            modalInstance.hide();

            selectedProjectId = null;
        });

        $(`${tableSelector} tbody`).on('click', 'tr', function (e) {
            if ($(e.target).closest('.dropdown, .dropdown-menu').length === 0) {
                window.location.href = '/projects/valuations';
            }
        });

        document.getElementById('addProjectBtn').onclick = function () {
            window.location.href = '/projects/add';
        };
    }
};

