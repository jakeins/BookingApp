import { Injectable } from '@angular/core';

@Injectable()
export class DevService  {

  static helperTerms = {
    'occasion' : ['Anniversary', 'Back to School', 'Baptism', 'Bar Mitzvah and Bat Mitzvah',
      'Birthday', 'Confirmation', 'Congratulations', 'Encouragement',
      'First Communion', 'Friendship', 'Get Well', 'Graduation', 'Military Appreciation',
      'New Baby', 'Quinceañera', 'Retirement', 'Sympathy', 'Teacher Appreciation',
      'Thank You', 'Wedding', 'Holidays', 'Armed Forces Day'],
    'wishAdj' : ['Good', 'Best', 'Own', 'Last', 'Unconscious', 'Personal', 'Sincere',
      'Individual', 'Warmest', 'Sexual', 'Ardent', 'Secret', 'Earnest', 'Pious', 'Repressed',
      'Well', 'Parental', 'Infantile', 'Longer', 'Cordial', 'Hearty', 'Oedipal', 'Kindest',
      'Incestuous', 'Warm', 'Fervent', 'Unfulfilled', 'Sincerest', 'Affectionate', 'Conscious'],
    'popNoun': ['Area', 'Book', 'Business', 'Case', 'Child', 'Company', 'Country', 'Day', 'Eye',
      'Fact', 'Family', 'Government', 'Group', 'Hand', 'Home', 'Job', 'Life', 'Lot', 'Man', 'Money',
      'Month', 'Mother', 'Mr', 'Night', 'Number', 'Part', 'People', 'Place', 'Point', 'Problem', 'Program'],
    'basicColor': ['Red', 'Orange', 'Yellow', 'Green', 'Blue', 'Purple', 'Brown', 'Magenta', 'Tan',
      'Cyan', 'Olive', 'Maroon', 'Navy', 'Aquamarine', 'Turquoise', 'Silver', 'Lime', 'Teal',
      'Indigo', 'Violet', 'Pink', 'Black', 'White', 'Gray'],
    'emojiNature': ['🐶', '🐱', '🐭', '🐹', '🐰', '🦊', '🦝', '🐻', '🐼', '🦘', '🦡', '🐨', '🐯', '🦁', '🐮', '🐷', '🐽', '🐸', '🐵', '🙈', '🙉', '🙊', '🐒', '🐔', '🐧', '🐦', '🐤', '🐣', '🐥', '🦆', '🦢', '🦅', '🦉', '🦚', '🦜', '🦇', '🐺', '🐗', '🐴', '🦄', '🐝', '🐛', '🦋', '🐌', '🐚', '🐞', '🐜', '🦗', '🕷', '🕸', '🦂', '🦟', '🦠', '🐢', '🐍', '🦎', '🦖', '🦕', '🐙', '🦑', '🦐', '🦀', '🐡', '🐠', '🐟', '🐬', '🐳', '🐋', '🦈', '🐊', '🐅', '🐆', '🦓', '🦍', '🐘', '🦏', '🦛', '🐪', '🐫', '🦙', '🦒', '🐃', '🐂', '🐄', '🐎', '🐖', '🐏', '🐑', '🐐', '🦌', '🐕', '🐩', '🐈', '🐓', '🦃', '🕊', '🐇', '🐁', '🐀', '🐿', '🦔', '🐾', '🐉', '🐲', '🌵', '🎄', '🌲', '🌳', '🌴', '🌱', '🌿', '☘️', '🍀', '🎍', '🎋', '🍃', '🍂', '🍁', '🍄', '🌾', '💐', '🌷', '🌹', '🥀', '🌺', '🌸', '🌼', '🌻', '🌞', '🌝', '🌛', '🌜', '🌚', '🌕', '🌖', '🌗', '🌘', '🌑', '🌒', '🌓', '🌔', '🌙', '🌎', '🌍', '🌏', '💫', '⭐️', '🌟', '✨', '⚡️', '☄️', '💥', '🔥', '🌪', '🌈', '☀️', '🌤', '⛅️', '🌥', '☁️', '🌦', '🌧', '⛈', '🌩', '🌨', '❄️', '☃️', '⛄️', '🌬', '💨', '💧', '💦', '☔️', '☂️', '🌊', '🌫']
  }

  public static RandomTerm(termType: string) {
    return this.helperTerms[termType][this.RandomTo(this.helperTerms[termType].length)];
  }



  public static GenerateText(words1st: number, words2nd?: number) {

    let wordsLeft : number;

    if (words2nd == undefined)
      wordsLeft = words1st;
    else
      wordsLeft = words1st + this.RandomTo(words2nd - words1st);
    
    let text = '';

    while (wordsLeft > 0) {
      let termsKeys = Object.keys(this.helperTerms);
      let termTypeRef = this.helperTerms[termsKeys[this.RandomTo(termsKeys.length)]];
      text += ' ' + termTypeRef[this.RandomTo(termTypeRef.length)];
      wordsLeft--;
    }
    return text;
  }

  public static RandomTo(from: number) {
    return Math.floor(Math.random() * from);
  }


}
