import { MyProjectTemplatePage } from './app.po';

describe('MyProject App', function() {
  let page: MyProjectTemplatePage;

  beforeEach(() => {
    page = new MyProjectTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
