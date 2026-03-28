$(document).ready(function () {
    $(document).on('click', '.tree-toggle', function (e) {
        e.stopPropagation();
        var $children = $(this).closest('li').children('ul');
        $children.slideToggle(300);

        var $icon = $(this).find('.node-icon');
        if ($icon.hasClass('fa-folder-open')) {
            $icon.removeClass('fa-folder-open').addClass('fa-folder');
        } else {
            $icon.removeClass('fa-folder').addClass('fa-folder-open');
        }
    });

    $(document).on('click', '#addChildItem', function () {
        var index = $('.child-item-row').length;
        var newRow = `
            <div class="child-item-row" data-index="${index}">
                <div class="row">
                    <div class="col-md-5">
                        <div class="form-group">
                            <label>Item Name</label>
                            <input type="text" name="ChildItems[${index}].ItemName" 
                                   class="form-control" placeholder="Enter child item name" required />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="form-group">
                            <label>Weight</label>
                            <input type="number" name="ChildItems[${index}].Weight" 
                                   class="form-control child-weight" placeholder="Enter weight" 
                                   min="0.01" step="0.01" required />
                        </div>
                    </div>
                    <div class="col-md-3 d-flex align-items-end">
                        <div class="form-group">
                            <button type="button" class="btn btn-danger btn-sm remove-child">
                                <i class="fas fa-trash"></i> Remove
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;

        $('#childItemsContainer').append(newRow);
        updateWeightCalculation();
    });

    $(document).on('click', '.remove-child', function () {
        $(this).closest('.child-item-row').remove();
        reindexChildItems();
        updateWeightCalculation();
    });

    $(document).on('input', '.child-weight', function () {
        updateWeightCalculation();
    });

    function updateWeightCalculation() {
        var parentWeight = parseFloat($('#parentWeight').val()) || 0;
        var existingChildWeight = parseFloat($('#existingChildWeight').val()) || 0;
        var newChildWeight = 0;

        $('.child-weight').each(function () {
            newChildWeight += parseFloat($(this).val()) || 0;
        });

        var totalUsed = existingChildWeight + newChildWeight;
        var remaining = parentWeight - totalUsed;

        $('#newChildWeightDisplay').text(newChildWeight.toFixed(2));
        $('#totalWeightDisplay').text(totalUsed.toFixed(2));
        $('#remainingWeightDisplay').text(remaining.toFixed(2));

        if (remaining < 0) {
            $('#remainingWeightDisplay').css('color', '#FC8181');
            $('#weightWarning').show();
        } else {
            $('#remainingWeightDisplay').css('color', '#4FD1C5');
            $('#weightWarning').hide();
        }
    }

    function reindexChildItems() {
        $('.child-item-row').each(function (index) {
            $(this).attr('data-index', index);
            $(this).find('input[name*="ItemName"]')
                .attr('name', 'ChildItems[' + index + '].ItemName');
            $(this).find('input[name*="Weight"]')
                .attr('name', 'ChildItems[' + index + '].Weight');
        });
    }

    if ($('#parentWeight').length) {
        updateWeightCalculation();
    }

    $(document).on('click', '.btn-delete', function (e) {
        if (!confirm('Are you sure you want to delete this item and all its children? This action cannot be undone.')) {
            e.preventDefault();
        }
    });

    setTimeout(function () {
        $('.alert').fadeOut(500);
    }, 5000);
});
