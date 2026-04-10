import { getDocumentRule } from './document-rules';

describe('getDocumentRule', () => {
  it('returns passport validation for international routes', () => {
    const rule = getDocumentRule('International');

    expect(rule.label).toBe('Passport Number');
    expect(rule.pattern).toBe('^[A-Z0-9]{6,9}$');
  });

  it('returns national id validation for domestic routes', () => {
    const rule = getDocumentRule('Domestic');

    expect(rule.label).toBe('National ID');
    expect(rule.pattern).toBe('^\\d{7,8}$');
  });
});