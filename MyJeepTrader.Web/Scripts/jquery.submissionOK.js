/* ------------------------------------------------------------------------
 * Plugin: submissionOK
 * Author: Ryan Molley
 * Company: Red Hawk Technologies (http://www.redhawk-tech.com)
 * Version: 4.0.2
 * Completion: February 14, 2016
 * ------------------------------------------------------------------------- */
(function (jQuery) {
    jQuery.fn.submissionOK = function (options) {
        numErrors = 0;

        var o = jQuery.extend({}, jQuery.fn.submissionOK.defaults, options, jQuery.expr[':'],
		{

		    init: function (item) {
		        jQuery('body *').removeClass('sok_errorAlert').removeClass('sok_errorBorder').removeClass('sok_noBorder').removeClass('sok_labelNoBorder');
		        if (jQuery('.sok_errorLabel').length > 0) 
					jQuery('.sok_errorLabel').remove();

		        var len = jQuery(item).length;

		        if (jQuery('[reqmask]').length > 0) 
					jQuery('[reqmask]').keyup();

		        jQuery(item).each(function (index) {
		            o.checkType(this);

		            if (index === len - 1) {
		                if (jQuery('[reqsub]')) {
		                    jQuery('[reqsub]').each(function () {
		                        o.checkType(null, this);
		                    });
		                }

		                if (numErrors === 0)
		                    !o.testMode ? jQuery.fn.submissionOK.formSubmission(o) : console.log('Form successfully submitted.');
		                else
		                    jQuery('body').trigger('ErrorOccurred');
		            }

		        });
		    },

		    checkType: function (item, subItem) {
		        var id_ = (subItem ? jQuery(subItem).attr('id') : jQuery(item).attr('req'));

		        if (jQuery('#' + id_).is('select'))
		            this.typeSelect(id_, item);
		        else if (jQuery('#' + id_).is('input[type="checkbox"]') || jQuery('#' + id_).is('input[type="radio"]'))
		            this.typeArray(id_, item);
		        else
		            this.typeText(id_, item, subItem);
		    },

		    typeArray: function (id_, item) {
		        var name = jQuery('#' + id_).attr('name');

		        if (name.indexOf("$") > 0) 
					name = name.substring(0, name.indexOf("$"));

		        var i = 0;

		        jQuery('[name^="' + name + '"]').each(function () {
		            if (jQuery(this).is(':checked'))
		                i += 1;
		        });

		        highlightErrors(o, id_, i, true, item);
		    },

		    typeSelect: function (id_, item) {
		        if (!jQuery('#' + id_ + 'option:selected') || jQuery('#' + id_).val() === this.selectFirstValue)
		            highlightErrors(o, id_, 0, false, item);
		    },

		    typeText: function (id_, item, subItem) {
		        if (subItem) {
		            var subVal = jQuery(subItem).attr('reqsub').split(':');
		            this.validateSub(id_, subItem, subVal[0], subVal[1]);
		        }
		        else {
		            if (jQuery('#' + id_).val() === '') 
						highlightErrors(o, id_, 0, false, item);
		        }

		        if (jQuery(item).attr('reqvalidate') && jQuery(item).val() !== '') {
		        	var valid = true;
		        	
		        	switch (jQuery(item).attr('reqvalidate').toLowerCase()) {
						case 'email':
							valid = jQuery.fn.submissionOK.email(jQuery(item).val());
							break;
						case 'password':
							valid = jQuery.fn.submissionOK.password(jQuery(item).val());
							break;
						case 'phone':
							valid = jQuery.fn.submissionOK.phone(jQuery(item).val());
							break;
						case 'zipcode':
							valid = jQuery.fn.submissionOK.zipcode(jQuery(item).val());
							break;
						default:
							valid = jQuery.fn.submissionOK.validation(jQuery(item).attr('reqvalidate'), jQuery(item).val());
					}
					
		            if (!valid)
		                highlightErrors(o, jQuery(item).attr('id'), 0, false, item, jQuery(item).attr('reqvalidate'));
		        }

		        if (jQuery(item).attr('reqconfirm')) {
		            if (jQuery('#' + jQuery(item).attr('reqconfirm')).val() !== jQuery(item).val())
		                highlightErrors(o, jQuery(item).attr('id'), 0, false, item, 'confirm');
		        }
		    },

		    validateSub: function (id_, item, reqId, reqValue) {
		        if (reqValue) {
		            if (jQuery('#' + reqId).is('input[type="checkbox"]') || jQuery('#' + reqId).is('input[type="radio"]'))
		                highlight(jQuery('#' + reqId + ':checked').val() === reqValue);
		            else
		                highlight(jQuery('#' + reqId).val() === reqValue);
		        }
		        else {
		            if (jQuery('#' + reqId).is('input[type^="select"]'))
		                highlight(jQuery('#' + reqId).val() !== this.selectFirstValue || jQuery('#' + reqId).is(':checked'));
		            else if (jQuery('#' + reqId).is('input[type="checkbox"]') || jQuery('#' + reqId).is('input[type="radio"]')) {
		                var name = jQuery('#' + reqId).attr('name');
		                highlight(jQuery('[name="' + name + '"]:checked').val());
		            }
		            else
		                highlight(jQuery('#' + reqId).val());
		        }

		        function highlight(parentValueCheck) {
		            if (parentValueCheck)
		                jQuery('#' + id_).val() !== '' ? highlightErrors(o, id_, 1, false, item) : highlightErrors(o, id_, 0, false, item, true);
		        }
		    }

		});

        o.init(this);
    };

    var numErrors = 0,
        errorMessages = {
            'default': 'is a required field.',
            'invalidFormat': 'is not in a valid format.',
            'captchaCheck': 'value does not appear to be valid, please try again.',
            'confirm': 'Cannot confirm, values do not match.'
        };

    jQuery(function () {
        jQuery('[reqmask]').each(function () {

            jQuery(this).keypress(function (e) {
                if (((e.keyCode ? e.keyCode : e.which) < 48 || (e.keyCode ? e.keyCode : e.which) > 57) &&
					((e.keyCode ? e.keyCode : e.which) !== 46 && (e.keyCode ? e.keyCode : e.which) !== 8) &&
					((e.keyCode ? e.keyCode : e.which) < 37 && (e.keyCode ? e.keyCode : e.which) > 40)) return false;
            });

            jQuery(this).keyup(function (e) {
                var non_mask = jQuery(this).val().split(''),
                reqmask = jQuery(this).attr('reqmask').split('');

                if ((e.keyCode ? e.keyCode : e.which) >= 37 && (e.keyCode ? e.keyCode : e.which) <= 40) return false;

                if (non_mask.join('') !== '') {

                    non_mask = jQuery.grep(non_mask, function (n) { return !isNaN(n) && n !== ' '; });

                    for (var i = 0; i < non_mask.length; i++) {

                        for (var j = i; j < reqmask.length; j++) {
                            if (reqmask[j] === '#' || reqmask[j] === non_mask[i]) break;
                            else {
                                non_mask.splice(i, 0, reqmask[j]);
                                i++;
                            }
                        }
                    }

                    if (non_mask.length > reqmask.length) non_mask = jQuery.grep(non_mask, function (n) { return !isNaN(n) && n !== ' '; });

                    jQuery(this).val(non_mask.join(''));
                }
            });

        });

        jQuery('[req], [reqsub]').each(function () {
            var id_ = (jQuery(this).attr('req') ? jQuery(this).attr('req') : jQuery(this).attr('id'));
			
            if (jQuery('#' + id_).is('input[type="checkbox"]') || jQuery('#' + id_).is('input[type="radio"]')) {
                var name = jQuery('#' + id_).attr('name');
				
                if (name.indexOf("$") > 0) 
					name = name.substring(0, name.indexOf("$"));

                jQuery('[name^="' + name + '"]').on('keyup change', function () {
                    removeSubmissionOkLabels(this, id_);
                });
            }
            else {
                jQuery('#' + id_).on('keyup change', function () {
                    removeSubmissionOkLabels(this, id_);
                });
            }
        });

        jQuery('[reqvalidate], [reqconfirm]').each(function () {
            jQuery(this).on('keyup change', function () {
                removeSubmissionOkLabels(this, jQuery(this).attr('id'));
            });
        });
		
		jQuery(document).on('keyup change', '[reqother]', function () {
			removeSubmissionOkLabels(this, jQuery(this).attr('id'));
		});
    });
    
    errorHandler = function (o, item) {
		if (jQuery(item).is('input[type="checkbox"]') || jQuery(item).is('input[type="radio"]')) {
			var name = jQuery(item).attr('name');
			
			if (name.indexOf("$") > 0) 
				name = name.substring(0, name.indexOf("$"));

			jQuery('[name^="' + name + '"]').each(function () {
				jQuery(this).attr('reqother', jQuery(item).attr('id'));
			});
		}
		else
			jQuery(item).attr('reqother', jQuery(item).attr('id'));
			
    	highlightErrors(o, jQuery(item).attr('id'), 0, (jQuery(item).is('input[type="checkbox"]') || jQuery(item).is('input[type="radio"]')), item, 'sok_other');
    };

    highlightErrors = function (o, id_, errorIndicatorValue, listArray, item, subItem) {
        var labelBorder;

        o.highlight ? labelBorder = 'sok_errorBorder' : (o.label ? labelBorder = 'sok_labelNoBorder' : labelBorder = 'sok_noBorder');

        if (errorIndicatorValue === 0) {
            addErrorHandlers();
            subItem ? errorMessage(o, labelBorder, item, subItem) : errorMessage(o, labelBorder, item);
        }

        function addErrorHandlers() {
			var node = jQuery('#' + id_).parent().get(0).tagName.toLowerCase();
			
            if (o.label) {
            	if (listArray) {
					switch (node) {
						case 'li':
							jQuery(item).closest('ul').addClass('sok_errorAlert');
							break;
						case 'td':
							jQuery(item).closest('table').addClass('sok_errorAlert');
							break;
						default:
							jQuery(item).parent().addClass('sok_errorAlert');
					}
                }
                else 
                	jQuery(item).addClass('sok_errorAlert');
			}

            if (jQuery('[reqhighlight="' + id_ + '"]').length)
                jQuery('[reqhighlight="' + id_ + '"]').addClass(labelBorder);
            else if (listArray) {
                switch (node) {
                    case 'li':
                        jQuery('#' + id_).closest('ul').addClass(labelBorder);
                        break;
                    case 'td':
                        jQuery('#' + id_).closest('table').addClass(labelBorder);
                        break;
                    default:
                        jQuery('#' + id_).parent().addClass(labelBorder);
                }
            }
            else
                jQuery('#' + id_).addClass(labelBorder);
        }

    };

    errorMessage = function (o, labelBorder, item, errorMessageType) {
        numErrors += 1;

        var reqName = jQuery(item).attr('reqname'),
			reqSub = jQuery(item).attr('reqsub'),
			reqError = jQuery(item).attr('reqerror'),
            reqValidate = jQuery(item).attr('reqvalidate'),
            reqConfirm = jQuery(item).attr('reqconfirm'),
			req = jQuery(item).attr('req');

        if (o.displayOnForm)
            inlineErrors();

        function inlineErrors() {
            var id_ = jQuery(item).attr('id');
            var $req, selection;

            selection = ((reqSub || reqValidate || reqConfirm) ? id_ : req);
            
            if (jQuery('[reqplaceholder="' + selection + '"]').length)
                $req = jQuery('[reqplaceholder="' + selection + '"]');
            else if (jQuery('#' + selection).next('label').length)
                $req = jQuery('#' + selection).next('label');
            else
                $req = jQuery('#' + selection).closest('.' + labelBorder);
            
            if (reqSub)
                reqError ? ($req.after('<div class="sok_errorLabel" reqlabel="' + id_ + '">' + reqError + '</div>')) : ($req.after('<div class="sok_errorLabel" reqlabel="' + id_ + '">' + (reqName ? reqName : (req ? req : reqSub)) + ' ' + errorMessages.default + '</div>'));
            else if (reqError) {
            	if (errorMessageType !== undefined) {
            		if (errorMessageType === 'sok_other')
            			jQuery(item).closest('.' + labelBorder).after('<div class="sok_errorLabel" reqlabel="' + jQuery(item).attr('id') + '">' + reqError + '</div>');
            		else
            			$req.after('<div class="sok_errorLabel" reqlabel="' + req + '">' + reqError + '</div>');
            	}
            	else
					$req.after('<div class="sok_errorLabel" reqlabel="' + req + '">' + reqError + '</div>');
			}
            else if (errorMessageType !== undefined) {
                switch (errorMessageType) {
                    case 'captcha':
                        $req.after('<div class="sok_errorLabel" reqlabel="' + req + '">' + (reqName ? reqName : req) + ' ' + errorMessages.captchaCheck + '</div>');
                        break;
                    case 'confirm':
                        $req.after('<div class="sok_errorLabel" reqlabel="' + jQuery(item).attr('id') + '">' + errorMessages.confirm + '</div>');
                        break;
                    case 'sok_other':
                    	jQuery(item).closest('.' + labelBorder).after('<div class="sok_errorLabel" reqlabel="' + jQuery(item).attr('id') + '">' + (reqName ? reqName : jQuery(item).attr('id')) + ' ' + errorMessages.default + '</div>');
                    	break;
                    default:
                        $req.after('<div class="sok_errorLabel" reqlabel="' + jQuery(item).attr('id') + '">' + (reqName ? reqName : jQuery(item).attr('id')) + ' ' + errorMessages.invalidFormat + '</div>');
                }
            }
            else
                $req.after('<div class="sok_errorLabel" reqlabel="' + req + '">' + (reqName ? reqName : (req ? req : reqSub)) + ' ' + errorMessages.default + '</div>');
        }
    };

    removeSubmissionOkLabels = function (item, id) {
        var node = jQuery('#' + id).parent().get(0).tagName.toLowerCase();

        switch (node) {
            case 'li':
                jQuery('#' + id).closest('ul').removeClass('sok_errorBorder').removeClass('sok_labelNoBorder').removeClass('sok_noBorder').removeClass('sok_errorAlert');
                break;
            case 'td':
                jQuery('#' + id).closest('table').removeClass('sok_errorBorder').removeClass('sok_labelNoBorder').removeClass('sok_noBorder').removeClass('sok_errorAlert');
                break;
        }

        jQuery('[req="' + id + '"]').removeClass('sok_errorBorder').removeClass('sok_labelNoBorder').removeClass('sok_noBorder').removeClass('sok_errorAlert');
        jQuery(item).removeClass('sok_errorBorder').removeClass('sok_labelNoBorder').removeClass('sok_noBorder').removeClass('sok_errorAlert');
        jQuery('[reqlabel="' + id + '"]').remove();
        jQuery('[reqhighlight="' + id + '"]').removeClass('sok_errorBorder').removeClass('sok_labelNoBorder').removeClass('sok_noBorder').removeClass('sok_errorAlert');
        jQuery('[reqplaceholder="' + id + '"]').next('.sok_errorLabel').remove();
		
		if (jQuery(item).attr('reqother')) {
			jQuery('[reqhighlight="' + jQuery(item).attr('reqother') + '"]').removeClass('sok_errorBorder').removeClass('sok_labelNoBorder').removeClass('sok_noBorder').removeClass('sok_errorAlert');
			jQuery('[reqplaceholder="' + jQuery(item).attr('reqother') + '"]').next('.sok_errorLabel').remove();
			jQuery('[reqlabel="' + jQuery(item).attr('reqother') + '"]').remove();
		}
    };
    
    jQuery.fn.submissionOK.email = function (value) {
    	var valid = /([a-zA-Z0-9_\-\.\+]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})/.test(value);
    	return valid;
    };
    
    jQuery.fn.submissionOK.password = function (value) {
    	var valid = /.{8,}/.test(value) && /[\"\#\$%!&\'\(\)\*\+,\-\.\/:;<=>\?@\[\\\]\^_\{\|\}~]/.test(value) && /[0-9]/.test(value);
    	return valid;
    };
    
    jQuery.fn.submissionOK.phone = function (value) {
        var valid = /^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$/gm.test(value);
    	return valid;
    };
    
    jQuery.fn.submissionOK.zipcode = function (value) {
    	var valid = /^([0-9]{5})$/.test(value) || /^([0-9]{5}-[0-9]{4})$/.test(value);
    	return valid;
    };

    jQuery.fn.submissionOK.validation = function (reqvalidate, value) {
    	var valid = true;
    	
    	switch (reqvalidate.toLowerCase()) {
    		case 'creditcard':
    			var re = new RegExp('^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|6(?:011|5[0-9]{2})[0-9]{12}|(?:2131|1800|35\d{3})\d{11})$');
				valid = re.test(value);
    			break; 
    		default:
    			break;
    	}
    	
    	return valid;
    };

    jQuery.fn.submissionOK.formSubmission = function (o) {
        if (o.submitButtonId !== null)
            jQuery('#' + o.submitButtonId).click();
        else {
			if (jQuery('form[reqform]').length > 0)
            	jQuery('form[reqform]').submit();
			else
				jQuery('form').eq(0).submit();
		}
    };

    jQuery.fn.submissionOK.defaults = {
        selectFirstValue: '',
        label: false,
        highlight: false,
        displayOnForm: true,
        submissionOKName: null,
        submitButtonId: null,
        testMode: false
    };

})(jQuery);
