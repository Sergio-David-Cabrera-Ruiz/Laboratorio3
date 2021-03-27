var grid = [];

async function AJAXSubmit(oFormElement) {
    const formData = new FormData(oFormElement);

    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const body = await response.json();

            Swal.fire(body.success ? 'Exito' : 'Error', body.message, body.success ? 'success' : 'error');

            if (body.success) {
                $('#fileUpload').val('');
                grid.search('');
                grid.ajax.reload();
            }
        }
        else {
            Swal.fire('Error', 'Ocurrio un error al subir el archivo', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire('Error','Ocurrio un error al subir el archivo','error');
    }
}

async function AJAXSubmitOrder(oFormElement) {
    const formData = new FormData(oFormElement);

    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const body = await response.json();

            Swal.fire(body.success ? 'Exito' : 'Error', body.message, body.success ? 'success' : 'error');

            if (body.success) {
                $('#grid-inventory tbody').html('');
                grid.search('');
                grid.ajax.reload();
            }
        }
        else {
            Swal.fire('Error', 'Ocurrio un error al confirmar el pedido', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire('Error','Ocurrio un error al confirmar el pedido','error');
    }
}

function actionsRender(data, type, row, meta) {
    return `<button class="btn btn-success btn-order" data-row='${meta.row}' data-max='${data.existencia}'>Pedir</button>`;
}

$(function () {
    grid = $('#grid').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": false,
        "searchDelay": 600,
        "ajax": {
            "url": "/Home/Grid",
            "type": "POST"
        },
        "columns": [
            { "data": "id", "searchable": false },
            { "data": "nombre", "searchable": true },
            { "data": "descripcion", "searchable": false },
            { "data": "casaProductora", "searchable": false },
            { "data": "precio", "searchable": false },
            { "data": "existencia", "searchable": false },
            { "data": null, "searchable": false }
        ],
        "columnDefs": [
            { "targets": 'grid-actions', 'render': actionsRender }
        ]
    });

    $('#grid').on('click', '.btn-order', function (e) {
        const rowId = $(this).attr('data-row');
        const maxValue = $(this).attr('data-max');

        Swal.fire({
            title: 'Confirmar orden',
            input: 'number',
            inputAttributes: {
                min: 1,
                max: maxValue
            },
            showCancelButton: true,
            confirmButtonText: 'Ordenar',
            showLoaderOnConfirm: true,
            preConfirm: (quantity) => {
                const data = grid.row(rowId).data();
                $('#grid-inventory tbody').append(`<tr><td>${data.id}<input type="hidden" name="data[${data.id}]" value="${quantity}"></td><td>${data.nombre}</td><td>${data.descripcion}</td><td>${data.casaProductora}</td><td>${data.precio}</td><td>${quantity}</td></tr>`);
            },
            allowOutsideClick: () => !Swal.isLoading()
        });
    });

    $('.btn-reset').click(function (e) {
        e.preventDefault();
        Swal.fire({
            title: "Reiniciar existencia del inventario",
            text: "Esta acción es permanente",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            confirmButtonText: "Si",
            cancelButtonText: "Cancelar",
            confirmButtonClass: 'red',
            showLoaderOnConfirm: true,
            allowOutsideClick: function () { return !swal.isLoading(); },
            preConfirm: function () {
                $.get('/Home/ResetInventory',
                    function (data) {
                        if (data.success) {
                            Swal.fire({
                                title: "Éxito",
                                text: "Inventario reabastecido",
                                type: "success",
                                showCancelButton: false,
                                confirmButtonColor: "#2979ff",
                                confirmButtonText: "Listo",
                                preConfirm: function () { grid.ajax.reload(); }
                            });
                        }
                        else {
                            Swal.fire('Error', 'Ocurrió un error al reabastecer el inventario', 'error');
                        }
                    },
                    'json');
            }
        });
    });
});