window.projectsEvents = {
    register: function (tableSelector, tableInstance) {
        $(tableSelector).on('click', '.edit-project', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            window.location.href = `/projects/edit?id=${id}`;
        });

        $(tableSelector).on('click', '.delete-project', async function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            //TODO add modal call from .razor

            try {
                await window.projectsApi.deleteProject(id);
                tableInstance.ajax.reload(null, false);
            } catch {
                alert('Failed to delete project');
            }
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
